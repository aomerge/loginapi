using Microsoft.AspNetCore.Authentication.JwtBearer;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.Json;

// crate a new project
var builder = WebApplication.CreateBuilder(args);

// Configura el servicio de Entity Framework Core
builder.Services.AddDbContext<modelContext>();

// Configurar la clave secreta para firmar y validar los tokens
var key = new byte[]
{
    0x9F, 0x8D, 0x7E, 0x4A, 0x56, 0x32, 0xB9, 0x1C,
    0x3F, 0x85, 0x2D, 0x7B, 0xE1, 0x90, 0xC4, 0x6F,
    0x21, 0x8E, 0xF7, 0x53, 0x06, 0x9A, 0x71, 0xB8,
    0x2C, 0x49, 0xD3, 0x5E, 0x76, 0xA2, 0x1F, 0x5D
};

// Agregar el servicio de autenticación
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Agregar el servicio de autorización
builder.Services.AddAuthorization();

// Configurar el servicio de CORS
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Router
app.MapGet("/", () => "¡Hola mundo!");

// crea una ruta name y que me devuelva el nombre que le pase por parametro pasado por el body
app.MapPost("/name", async (HttpRequest request) =>
{
    using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
    {
        string bodyContent = await reader.ReadToEndAsync();

        dynamic parsedBody = JObject.Parse(bodyContent);
        string? name = parsedBody.name;

        return $"Hola {name}";
    }
});

// se crea la ruta register para registrar un usuario
app.MapPost("/register", async (HttpContext http) =>
{
    var formCollection = await http.Request.ReadFormAsync();
    string? Name = formCollection["name"];
    string? Password = formCollection["password"];
    string? Email = formCollection["email"];
    string? User_handle = formCollection["user_handle"];
    try
    {
        // Comprobar si el usuario o la contraseña están vacíos
        if( string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Password))
        {
            await Results.BadRequest("El usuario o la contraseña no pueden estar vacíos").ExecuteAsync(http);
            return;    
        }
        // encriptamos la contraseña
        string? hash = BCryptNet.HashPassword(Password);

        // generar el token con el nombre y la contraseña
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Hash, hash)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // guardar el usuario en la base de datos
        var user = new User
        {
            Username = Name,
            Password = hash,
            Email = Email,
            user_handle = User_handle
        };
        using var db = new modelContext();
        db?.Users?.Add(user);
        db?.SaveChanges();

        // devolver el token
        await Results.Ok(tokenString).ExecuteAsync(http);
    }
    catch (Exception e)
    {
        await Results.BadRequest(e.Message).ExecuteAsync(http);
    }
});

/* Ruta Login
* 1. Comprobar si el usuario o la contraseña están vacíos
* 2. Comprobar si el usuario existe en la base de datos
* 3. Comprobar si la contraseña es correcta
* 4. Generar el token
* 5. Devolver el token
*/
app.MapPost("/login", async (HttpContext http) =>
{
    var formCollection = await http.Request.ReadFormAsync();
    string? Name = formCollection["name"];
    string? Password = formCollection["password"];
    try
    {
        // Comprobar si el usuario o la contraseña están vacíos
        if( string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Password))
        {
            await Results.BadRequest("El usuario o la contraseña no pueden estar vacíos").ExecuteAsync(http);
            return;    
        }
        // encriptamos la contraseña
        string? hash = BCryptNet.HashPassword(Password);

        // generar el token con el nombre y el password
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Hash, hash),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddDays(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var tokenString = tokenHandler.WriteToken(token);

        // status code 200
        http.Response.StatusCode = 200;
        await http.Response.WriteAsync(tokenString);
    }catch
    {
        await Results.BadRequest("El usuario o la contraseña no son correctos").ExecuteAsync(http);
        return;
    }
});

// comprueva si el tokend es valido
app.MapPost("/validate", async (HttpContext http) =>
{
    // Recogemos el token en el encabezado "Authorization" con el tipo "Bearer"
    string? token = http.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

    // Comprobamos si el token es válido
    var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        http.Response.StatusCode = 200;
        // Devuelve el token desencriptado
        await http.Response.WriteAsync(tokenHandler.ReadJwtToken(token).ToString());
    }
    catch
    {
        http.Response.StatusCode = 401;
        await http.Response.WriteAsync("Token inválido");
    }
});

// se integra la ruta user
// la cual muestra el nombre del usuario y comprueva el token caso contrario manda un error 401
app.MapGet("/user", async (HttpContext http) =>
{
    // Recogemos el token en el encabezado "Authorization" con el tipo "Bearer"
    string? token = http.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");


    // Comprobamos si el token es válido
    var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        http.Response.StatusCode = 200;
        // Devuelve el token desencriptado
        await http.Response.WriteAsync(tokenHandler.ReadJwtToken(token).ToString());
    }
    catch
    {
        http.Response.StatusCode = 401;
        await http.Response.WriteAsync("Token inválido");
    }
});


app.Run();

public class RequestBody
{
    public string? Nombre { get; set; }
}
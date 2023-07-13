/*
* Autor: Ángel Ortega
* Name: loginapi
* Fecha: 10/10/2021
* Descripción: Este archivo contiene el código de la API de inicio de sesión
* Tecnologias: .NET 7.0, jwt, bcrypt, entity framework core, sql server
*/
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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// crate a new project
var builder = WebApplication.CreateBuilder(args);

// Configurar la conexión a la base de datos MariaDB
    var connectionString = "Server=localhost;Port=3306;Database=mydatabase;Uid=myuser;Pwd=mypassword;";
    builder.Services.AddDbContext<modelContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 5, 12)))
            .EnableSensitiveDataLogging() // Habilitar registros detallados
            .LogTo(Console.WriteLine)); // Agregar registro de mensajes a la consola

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
    // migaration
    /* using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<modelContext>();

        try
        {
            // Aplicar migraciones y crear las tablas en la base de datos si no existen
            dbContext.Database.Migrate();
            Console.WriteLine("Migraciones aplicadas y tablas creadas correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al aplicar migraciones y crear las tablas: {ex.Message}");
        }
    } */

    app.UseAuthentication();
    app.UseAuthorization();

// Router
    app.MapGet("/", () => "¡Hola mundo!");

/* Ruta de inicio de sesión
* 1 - Leer los datos del formulario
* 2 - Obtener los datos del formulario
* 3 - Comprobar si el usuario o la contraseña están vacíos
* 4 - Comprobar si el usuario existe en la base de datos
* 5 - Comprobar si la contraseña es correcta
* 6 - Comprobar si el correo electrónico está verificado
* 7 - Generar un token de acceso
* 8 - Devolver una respuesta exitosa
*/
    app.MapPost("/register", async ( modelContext dbContext, HttpContext http) =>
    {
        try
        {
            // Leer los datos del formulario
                var formCollection = await http.Request.ReadFormAsync();
                // --
            // Obtener los datos del formulario
                string? _Name = formCollection["name"];
                string? Password = formCollection["password"];
                string? _Email = formCollection["email"];
                string? _User_handle = formCollection["user_handle"];
                // --
            // Encriptamos la contraseña
                string? hash = BCryptNet.HashPassword(Password);
                // --
            // Comprobar si el usuario o la contraseña están vacíos
                if( string.IsNullOrEmpty(_Name) || string.IsNullOrEmpty(Password))
                {
                    await Results.BadRequest("El usuario o la contraseña no pueden estar vacíos").ExecuteAsync(http);
                    return;    
                }
                // --
            // Comprobar si el usuario existe en la base de datos
                bool userExists = dbContext.Users.Any(u => u.Email == _Email || u.user_handle == _User_handle);
                // verificar si el usuario existe
                if (userExists)
                {
                    // Devolver un error de conflicto si el usuario ya existe
                        http.Response.StatusCode = StatusCodes.Status409Conflict;
                        await http.Response.WriteAsync("El usuario ya existe");
                        return;
                }
                else 
                {
                    // guardar los datos del usuario en mi base de datos
                        User user = new User
                        {
                            Username = _Name,
                            Password = hash,
                            Email = _Email,
                            user_handle = _User_handle
                        };
                        dbContext?.Users?.Add(user);
                        dbContext?.SaveChanges();   
                    // guardamos los datos de la verificacion en la base de datos y interamos el user_id                                         
                        var userId = dbContext?.Users?.FirstOrDefault(x => x.Email == _Email);
                        int user_id = userId.Id_user;
                        VerificationUser verification = new VerificationUser
                        {
                            UserId = user_id,
                            Verified = false
                        };
                        dbContext?.VerificationUsers?.Add(verification);
                        dbContext?.SaveChanges();   

                    // Generar un enlace de verificación único
                        var linkGenerator = new VerificationLinkGenerator();
                        string verificationLink = linkGenerator.GenerateVerificationLink(_Name, _Email, _User_handle, hash );
                    // Enviar el correo electrónico de verificación
                        var emailService = new EmailService();
                        emailService.SendVerificationEmail(_Email, verificationLink);
                    // --
                }
                // --
            // Devolver una respuesta exitosa
                http.Response.StatusCode = 200;
                await http.Response.WriteAsync("Enviamos un correo electrónico de verificación a su dirección de correo electrónico. Por favor, verifique su cuenta para iniciar sesión.");
                // --
        }
        catch (Exception e)
        {
            // Devolver un error de servidor si ocurre un error
                await Results.BadRequest(e.Message).ExecuteAsync(http);
                // --
        }
    });
    // --

/* Ruta de verificación
* 1. Comprobar si el token es válido
* 2. Comprobar si el usuario existe en la base de datos
* 3. Activar la cuenta
* 4. Devolver una respuesta exitosa
*/
    app.MapGet("/verify", async (modelContext dbContext ,HttpContext http, string code) =>
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {            
            // Verificar el token
                var jwtToken = tokenHandler.ReadJwtToken(code);
                var jsonPayload = jwtToken.Payload.SerializeToJson();
                // --
            // Puedes deserializar el objeto JSON en un diccionario o en una clase personalizada
                var payloadDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonPayload);

            // Acceder a los valores del token
                string? email = payloadDictionary?["email"].ToString();
                string? userHandle = payloadDictionary?["nameid"].ToString();
            // Comprobar si el token es válido
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    throw new Exception("El token ha expirado");
                }
                // --         
            // comprueva si el usuario se encuentra verificado
                var user = dbContext?.Users?.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                        http.Response.StatusCode = StatusCodes.Status409Conflict;
                        await http.Response.WriteAsync($"El usuario no existe");
                        return;
                }
                var verification = dbContext?.VerificationUsers?.FirstOrDefault(x => x.UserId == user.Id_user);
                if (verification?.Verified == true)
                {
                        http.Response.StatusCode = StatusCodes.Status409Conflict;
                        await http.Response.WriteAsync($"El usuario ya se encuentra verificado");
                        return;
                }
                // --
            // Activar la cuenta        
                verification.Verified = true;
                dbContext?.SaveChanges(); 
                // --
            // Devolver una respuesta exitosa        
                await Results.Ok($"Cuenta activada exitosamente").ExecuteAsync(http);
                // --
        }
        catch (Exception e)
        {
            // Devolver un error de servidor si ocurre un error
                await Results.BadRequest(e.Message).ExecuteAsync(http);
                return;
            // --
        }
    });

/* ruta de reenvio de correo de verificacion
* 1. Comprobar si el usuario existe en la base de datos
* 2. Generar un enlace de verificación único
* 3. Enviar el correo electrónico de verificación
*/
    app.MapGet("/resend", async (modelContext dbContext ,HttpContext http, string email) =>
    {
        try
        {
            // Comprobar si el usuario existe en la base de datos
                var user = dbContext?.Users?.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                        http.Response.StatusCode = StatusCodes.Status409Conflict;
                        await http.Response.WriteAsync($"El usuario no existe");
                        return;
                }
                // --
            // Generar un enlace de verificación único
                var linkGenerator = new VerificationLinkGenerator();
                string verificationLink = linkGenerator.GenerateVerificationLink(user.Username, user.Email, user.user_handle, user.Password );
            // Enviar el correo electrónico de verificación
                var emailService = new EmailService();
                emailService.SendVerificationEmail(user.Email, verificationLink);
            // --
            // Devolver una respuesta exitosa
                await Results.Ok($"Enviamos un correo electrónico de verificación a su dirección de correo electrónico. Por favor, verifique su cuenta para iniciar sesión.").ExecuteAsync(http);
                // --
        }
        catch (Exception e)
        {
            // Devolver un error de servidor si ocurre un error
                await Results.BadRequest(e.Message).ExecuteAsync(http);
                return;
            // --
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
        try
        {
            // Leer los datos del formulario
                var formCollection = await http.Request.ReadFormAsync();
                string? Name = formCollection["name"];
                string? Password = formCollection["password"];
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
            // bad request
                await Results.BadRequest("El usuario o la contraseña no son correctos").ExecuteAsync(http);
                return;
        }
    });

/* Ruta para obtener el usuario  
* 1. Recoger el token
* 2. Comprobar si el token es válido
* 3. Devolver el token desencriptado
*/
    app.MapGet("/user", async (HttpContext http) =>
    {
        try
        {
            // Recogemos el token en el encabezado "Authorization" con el tipo "Bearer"
                string? token = http.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            // Comprobamos si el token es válido
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);
                var ab= tokenHandler.ReadToken(token);

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

/* Ruta para recuperar la contraseña
* 1. Comprobar si el usuario existe en la base de datos
* 2. Generar un enlace de recuperación de contraseña único
*/
    app.MapPost("/recover", async (modelContext dbContext, HttpContext http, string email) =>
    {
        try
        {
            // Comprobar si el usuario existe en la base de datos
                var user = dbContext?.Users?.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                        http.Response.StatusCode = StatusCodes.Status409Conflict;
                        await http.Response.WriteAsync($"El usuario no existe");
                        return;
                }
                // --
            // Generar un enlace de recuperación de contraseña único
                var linkGenerator = new VerificationLinkGenerator();
                string verificationLink = linkGenerator.GenerateVerificationLink(user.Username, user.Email, user.user_handle, user.Password );
            // --
            // Devolver una respuesta exitosa
                await Results.Ok($"Enviamos un correo electrónico de recuperación de contraseña a su dirección de correo electrónico. Por favor, verifique su cuenta para iniciar sesión.").ExecuteAsync(http);
                // --
        }
        catch (Exception e)
        {
            // Devolver un error de servidor si ocurre un error
                await Results.BadRequest(e.Message).ExecuteAsync(http);
                return;
            // --
        }
    });

/* Ruta de cambio de contraseña
* 1. Comprobar si el usuario existe en la base de datos
* 2. Comprobar si la contraseña es correcta
* 3. Comprovar si la contraseña antigua es correcta
*/
    app.MapPost("/resent", ()=>{
    
    });
// Ejecutar la aplicación
    app.Run();
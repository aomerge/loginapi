using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public class VerificationLinkGenerator
{
  // creamos un constructor para poder usar la configuración de la aplicación
    private readonly IConfiguration _configuration;

    public VerificationLinkGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }
  // creamos un motodo para generar el token
    public string GenerateToken( string Name, string Email, string UserHandler, string Password )
    {
      // generar el token con el nombre, el email, el user_handler y la contraseña
          string? keyJwt = _configuration.GetValue<string>("Jwt:Key");
          Byte[] key =  Convert.FromBase64String(keyJwt);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.NameIdentifier, UserHandler),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Hash, Password),
                new Claim(ClaimTypes.Role,"Admin")
            }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(token);
        return tokenString;
      }
  // creamos un metodo para generar el enlace de verificación    
    public string GenerateVerificationLink(string Name, string Email, string UserHandler, string Password)
    {
      // Generar el código de verificación
      const string? chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
      var random = new Random();
      string verificationCode = new string(Enumerable.Repeat(chars, 10)
        .Select(s => s[random.Next(s.Length)]).ToArray());

      string? Token = this.GenerateToken(Name, Email, UserHandler, Password);

      // Construir el enlace de verificación
      string verificationLink = $"https://localhost:5103/verify?code={Token}";

      return verificationLink;
    }
}

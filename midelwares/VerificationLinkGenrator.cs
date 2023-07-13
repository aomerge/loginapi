using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public class VerificationLinkGenerator
{
  // creamos un motodo para generar el token
    public string GenerateToken( string Name, string Email, string UserHandler, string Password )
    {
        // generar el token con el nombre, el email, el user_handler y la contraseña
        var key = new byte[]
        {
            0x9F, 0x8D, 0x7E, 0x4A, 0x56, 0x32, 0xB9, 0x1C,
            0x3F, 0x85, 0x2D, 0x7B, 0xE1, 0x90, 0xC4, 0x6F,
            0x21, 0x8E, 0xF7, 0x53, 0x06, 0x9A, 0x71, 0xB8,
            0x2C, 0x49, 0xD3, 0x5E, 0x76, 0xA2, 0x1F, 0x5D
        };

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

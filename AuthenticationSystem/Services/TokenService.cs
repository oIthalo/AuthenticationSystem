using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace AuthenticationSystem.Services;

public static class TokenService
{
    public static string GenerateToken(User model)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var key = Encoding.ASCII.GetBytes(secretKey!);

        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

        var roleName = model.Role?.Name?.ToLower() ?? "User";

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, roleName),
            }),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
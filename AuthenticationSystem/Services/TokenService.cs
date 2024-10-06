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
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(model),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static ClaimsIdentity GenerateClaims(User model)
    {
        var ci = new ClaimsIdentity();

        ci.AddClaim(new Claim(ClaimTypes.Name, model.Email));
        ci.AddClaim(new Claim(ClaimTypes.Role, model.Role.ToString()!));

        return ci;
    }
}
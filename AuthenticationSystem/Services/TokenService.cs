using AuthenticationSystem.Data;
using AuthenticationSystem.Interfaces;
using AuthenticationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace AuthenticationSystem.Services;

public class TokenService : ITokenService
{
    private AppDbContext _context;

    public TokenService(AppDbContext context)
        => _context = context;

    public string GenerateToken(User model)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var key = Encoding.ASCII.GetBytes(secretKey!);

        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, model!.Id.ToString()),
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, model.Role.Name ?? "User"),
            }),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var key = Encoding.ASCII.GetBytes(secretKey!);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Expires = DateTime.UtcNow.AddHours(2),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var key = Encoding.UTF8.GetBytes(secretKey!);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Algoritmo de assinatura inválido");

        return principal;
    }

    public void SaveRefreshToken(string username, string token)
    {
        var refreshToken = _context.RefreshTokens.FirstOrDefault(x => x.TokenRefresh == token);
        _context.RefreshTokens.Add(new RefreshToken { Id = Guid.NewGuid(), Username = username, TokenRefresh = token });
        _context.SaveChanges();
    }

    public string GetRefreshToken(string username)
    {
        var refreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Username == username);
        return refreshToken!.TokenRefresh;
    }

    public void DeleteRefreshToken(string username, string token)
    {
        var refreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Username == username);
        _context.RefreshTokens.Remove(refreshToken!);
        _context.SaveChanges();
    }

    public void Logout(string username)
    {
        var refreshToken = _context.RefreshTokens.Where(x => x.Username == username);
        
        if (refreshToken.Any())
        {
            _context.RefreshTokens.RemoveRange(refreshToken);
            _context.SaveChanges();
        }
    }
}
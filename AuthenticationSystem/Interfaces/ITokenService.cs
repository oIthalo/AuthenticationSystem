using AuthenticationSystem.Models;
using System.Security.Claims;
namespace AuthenticationSystem.Interfaces;

public interface ITokenService
{
    string GenerateToken(User model);
    string GenerateToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    void SaveRefreshToken(string username, string token);
    string GetRefreshToken(string username);
    void DeleteRefreshToken(string username, string token);
    void Logout(string username);
}
using AuthenticationSystem.Models;
namespace AuthenticationSystem.Data.DataResponses;

public class ResponseLogin
{
    public string Username { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
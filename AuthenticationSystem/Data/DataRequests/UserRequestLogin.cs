namespace AuthenticationSystem.Data.DataRequests;

public class UserRequestLogin
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
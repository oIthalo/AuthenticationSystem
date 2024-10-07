namespace AuthenticationSystem.Data.DataRequests;

public class UserRequestRegister
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string RePassword { get; set; }
}
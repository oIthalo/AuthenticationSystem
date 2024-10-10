namespace AuthenticationSystem.Data.DataRequests;

public class ResetPasswordRequest
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
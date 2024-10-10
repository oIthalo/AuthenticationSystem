using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Data.DataRequests;

public class ForgotPasswordRequest
{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}
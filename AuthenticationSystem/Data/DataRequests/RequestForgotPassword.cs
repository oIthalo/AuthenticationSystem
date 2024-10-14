using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Data.DataRequests;

public class RequestForgotPassword
{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}
using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Data.DataRequests;

public class RequestForgotPasssword
{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}
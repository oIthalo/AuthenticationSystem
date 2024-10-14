using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Data.DataRequests;

public class RequestLogin
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
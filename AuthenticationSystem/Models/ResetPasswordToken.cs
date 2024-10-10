using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Models;

public class ResetPasswordToken
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public bool IsUsed { get; set; }
}
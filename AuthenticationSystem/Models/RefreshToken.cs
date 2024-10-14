using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Models;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public string TokenRefresh { get; set; }
}
using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Models;

public class User
{
    [Key]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Field is required")]
    [StringLength(16, ErrorMessage = "Username can't be longer than 16 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [StringLength(16, ErrorMessage = "Password can't be longer than 16 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [Compare(nameof(Password), ErrorMessage = "Password missmatch")]
    public string RePassword { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public Guid RoleId { get; set; }

    public Role Role { get; set; }
}
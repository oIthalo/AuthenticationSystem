using System.ComponentModel.DataAnnotations;
namespace AuthenticationSystem.Models;

public class Role
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [StringLength(32, ErrorMessage = "Role can't be longer than 32 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [StringLength(300, ErrorMessage = "Description can't be longer than 300 characters")]
    public string Description { get; set; }

    public List<User> Users { get; set; }
}
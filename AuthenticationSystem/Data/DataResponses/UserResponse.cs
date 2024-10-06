namespace AuthenticationSystem.Data.DataResponses;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public RoleResponse Role { get; set; }
}
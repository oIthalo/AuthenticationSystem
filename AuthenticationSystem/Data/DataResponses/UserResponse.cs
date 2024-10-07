using System.Text.Json.Serialization;
namespace AuthenticationSystem.Data.DataResponses;

public class UserResponse
{
    [JsonPropertyName("nameid")]
    public Guid Id { get; set; }
    [JsonPropertyName("unique_name")]
    public string Username { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }
}
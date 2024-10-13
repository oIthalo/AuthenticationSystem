using System.Text.Json.Serialization;
namespace AuthenticationSystem.Data.DataResponses;

public class ResponseUser
{
    [JsonPropertyName("nameid")]
    public Guid Id { get; set; }
    [JsonPropertyName("unique_name")]
    public string Username { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }
}
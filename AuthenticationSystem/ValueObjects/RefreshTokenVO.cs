using System.Text.Json.Serialization;
namespace AuthenticationSystem.ValueObjects
{
    public class RefreshTokenVO
    {
        public RefreshTokenVO(string tokenRefresh)
        {
            TokenRefresh = tokenRefresh;
        }

        [JsonPropertyName("RefreshToken")]
        public string TokenRefresh { get; set; }

        public override string ToString() => TokenRefresh;
    }
}

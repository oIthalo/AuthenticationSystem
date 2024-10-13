using System.Text.Json.Serialization;

namespace AuthenticationSystem.ValueObjects
{
    public class TokenJwtVO
    {
        public TokenJwtVO(string jwtToken)
        {
            JwtToken = jwtToken;
        }

        [JsonPropertyName("TokenJwt")]
        public string JwtToken { get; set; }

        public override string ToString() => JwtToken;
    }
}

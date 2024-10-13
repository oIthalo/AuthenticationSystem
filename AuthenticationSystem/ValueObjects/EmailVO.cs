using System.Text.Json.Serialization;

namespace AuthenticationSystem.ValueObjects
{
    public class EmailVO
    {
        public EmailVO(string email)
        {
            Email = email;
        }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        public override string ToString() => Email;
    }
}

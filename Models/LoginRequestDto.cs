using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class LoginRequestDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("passwordHash")]
        public string Password { get; set; }
    }
} 
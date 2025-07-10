using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class RegisterRequestDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; }
    }
} 
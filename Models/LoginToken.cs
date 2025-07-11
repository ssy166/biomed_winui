using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class LoginToken
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        
        [JsonPropertyName("csrf_token")]
        public string CsrfToken { get; set; }
    }
} 
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class ReviewRequestDto
    {
        [JsonPropertyName("applicationId")]
        public int ApplicationId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("reviewComment")]
        public string ReviewComment { get; set; }
    }
} 
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class EduVideo
    {
        [JsonPropertyName("videoId")]
        public long VideoId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("displayOrder")]
        public int DisplayOrder { get; set; }
    }
} 
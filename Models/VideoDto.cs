using System;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class VideoDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; }

        [JsonPropertyName("coverUrl")]
        public string CoverUrl { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("uploaderId")]
        public long UploaderId { get; set; }

        [JsonPropertyName("uploaderName")]
        public string UploaderName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        // 便于UI显示的格式化属性
        public string FormattedDuration
        {
            get
            {
                var timeSpan = TimeSpan.FromSeconds(Duration);
                if (timeSpan.TotalHours >= 1)
                {
                    return $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                return $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}";
            }
        }
    }
} 
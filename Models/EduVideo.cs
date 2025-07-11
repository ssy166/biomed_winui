using System;
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
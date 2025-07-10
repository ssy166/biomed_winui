using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class EduResourceDetail
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("authorName")]
        public string AuthorName { get; set; }

        [JsonPropertyName("coverImageUrl")]
        public string CoverImageUrl { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("videos")]
        public List<EduVideo> Videos { get; set; }
    }
} 
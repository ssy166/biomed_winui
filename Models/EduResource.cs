using System;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class EduResource
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("authorName")]
        public string AuthorName { get; set; }

        [JsonPropertyName("coverImageUrl")]
        public string CoverImageUrl { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }
    }
} 
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace biomed.Models
{
    public class EduCategory
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
} 
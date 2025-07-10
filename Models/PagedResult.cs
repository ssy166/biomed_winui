using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class PagedResult<T>
    {
        [JsonPropertyName("records")]
        public List<T> Records { get; set; } = new List<T>();

        [JsonPropertyName("total")]
        public long Total { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("current")]
        public int Current { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }
    }
} 
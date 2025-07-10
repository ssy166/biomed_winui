using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class EduPagedResult<T>
    {
        [JsonPropertyName("content")]
        public List<T> Content { get; set; }

        [JsonPropertyName("totalElements")]
        public long TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }
    }
} 
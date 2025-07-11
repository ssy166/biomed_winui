using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class VideoPagedResult
    {
        [JsonPropertyName("content")]
        public List<VideoDto> Content { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalElements")]
        public long TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("last")]
        public bool Last { get; set; }

        [JsonPropertyName("first")]
        public bool First { get; set; }
    }
} 
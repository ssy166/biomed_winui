using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class FormulaPagedResult
    {
        [JsonPropertyName("records")]
        public List<Formula> Records { get; set; } = new List<Formula>();

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
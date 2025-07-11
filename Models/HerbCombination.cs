using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class HerbCombination
    {
        [JsonPropertyName("herbName")]
        public string HerbName { get; set; }

        [JsonPropertyName("combinationCount")]
        public long CombinationCount { get; set; }

        [JsonPropertyName("combinationRatio")]
        public double CombinationRatio { get; set; }

        // UI辅助属性
        public string CombinationRatioPercentage => $"{(CombinationRatio * 100):F0}%";
    }
} 
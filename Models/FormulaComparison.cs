using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class FormulaComparison
    {
        [JsonPropertyName("formulas")]
        public List<Formula> Formulas { get; set; } = new List<Formula>();

        [JsonPropertyName("comparisonPoints")]
        public Dictionary<string, List<string>> ComparisonPoints { get; set; } = new Dictionary<string, List<string>>();
    }
} 
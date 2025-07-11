using System;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class Formula
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("dynasty")]
        public string Dynasty { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }

        [JsonPropertyName("composition")]
        public string Composition { get; set; }

        [JsonPropertyName("preparation")]
        public string Preparation { get; set; }

        [JsonPropertyName("usage")]
        public string Usage { get; set; }

        [JsonPropertyName("dosageForm")]
        public string DosageForm { get; set; }

        [JsonPropertyName("functionEffect")]
        public string FunctionEffect { get; set; }

        [JsonPropertyName("mainTreatment")]
        public string MainTreatment { get; set; }

        [JsonPropertyName("clinicalApplication")]
        public string ClinicalApplication { get; set; }

        [JsonPropertyName("pharmacologicalAction")]
        public string PharmacologicalAction { get; set; }

        [JsonPropertyName("contraindication")]
        public string Contraindication { get; set; }

        [JsonPropertyName("caution")]
        public string Caution { get; set; }

        [JsonPropertyName("modernResearch")]
        public string ModernResearch { get; set; }

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }
    }
} 
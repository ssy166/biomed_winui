using System;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class ResearchApplication
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("projectId")]
        public int ProjectId { get; set; }

        [JsonPropertyName("studentId")]
        public int StudentId { get; set; }

        [JsonPropertyName("applicationReason")]
        public string ApplicationReason { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("reviewedBy")]
        public int? ReviewedBy { get; set; }

        [JsonPropertyName("reviewedAt")]
        public DateTime? ReviewedAt { get; set; }

        [JsonPropertyName("reviewComment")]
        public string ReviewComment { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("projectName")]
        public string ProjectName { get; set; }

        [JsonPropertyName("studentName")]
        public string StudentName { get; set; }
    }
} 
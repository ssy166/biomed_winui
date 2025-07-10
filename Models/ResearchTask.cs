using System;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class ResearchTask
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("projectId")]
        public int ProjectId { get; set; }

        [JsonPropertyName("studentId")]
        public int StudentId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("requirements")]
        public string Requirements { get; set; }

        [JsonPropertyName("deadline")]
        public string Deadline { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("progress")]
        public double Progress { get; set; }

        [JsonPropertyName("studentName")]
        public string StudentName { get; set; }

        [JsonPropertyName("projectName")]
        public string ProjectName { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
} 
using System;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class ResearchSubmission
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("taskId")]
        public int TaskId { get; set; }

        [JsonPropertyName("studentId")]
        public int StudentId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("abstractText")]
        public string AbstractText { get; set; }

        [JsonPropertyName("keywords")]
        public string Keywords { get; set; }

        [JsonPropertyName("fileUrl")]
        public string FileUrl { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("submissionNotes")]
        public string SubmissionNotes { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("submissionTime")]
        public DateTime SubmissionTime { get; set; }

        [JsonPropertyName("studentName")]
        public string StudentName { get; set; }

        [JsonPropertyName("taskTitle")]
        public string TaskTitle { get; set; }
    }
} 
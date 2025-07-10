using System;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class ResearchProject
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("projectName")]
        public string ProjectName { get; set; }

        [JsonPropertyName("projectCode")]
        public string ProjectCode { get; set; }

        [JsonPropertyName("projectType")]
        public string ProjectType { get; set; }

        [JsonPropertyName("fundingSource")]
        public string FundingSource { get; set; }

        [JsonPropertyName("fundingAmount")]
        public decimal FundingAmount { get; set; }

        [JsonPropertyName("principalInvestigatorName")]
        public string PrincipalInvestigatorName { get; set; }

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public string EndDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("researchField")]
        public string ResearchField { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("memberCount")]
        public int MemberCount { get; set; }

        [JsonPropertyName("taskCount")]
        public int TaskCount { get; set; }

        [JsonPropertyName("abstractText")]
        public string AbstractText { get; set; }

        [JsonPropertyName("keywords")]
        public string Keywords { get; set; }
    }
} 
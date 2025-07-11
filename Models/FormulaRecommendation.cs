using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace biomed.Models
{
    public class FormulaRecommendation : INotifyPropertyChanged
    {
        [JsonPropertyName("formulaId")]
        public long FormulaId { get; set; }

        [JsonPropertyName("formulaName")]
        public string FormulaName { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("matchedSymptoms")]
        public List<string> MatchedSymptoms { get; set; } = new List<string>();

        [JsonPropertyName("recommendation")]
        public string Recommendation { get; set; }

        // UI辅助属性
        private Formula _details;
        public Formula Details
        {
            get => _details;
            set
            {
                if (_details != value)
                {
                    _details = value;
                    OnPropertyChanged(nameof(Details));
                }
            }
        }

        private bool _isLoadingDetails;
        public bool IsLoadingDetails
        {
            get => _isLoadingDetails;
            set
            {
                if (_isLoadingDetails != value)
                {
                    _isLoadingDetails = value;
                    OnPropertyChanged(nameof(IsLoadingDetails));
                }
            }
        }

        public string ScorePercentage => $"{(Score * 100):F1}%";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SymptomRequest
    {
        [JsonPropertyName("symptoms")]
        public List<string> Symptoms { get; set; } = new List<string>();
    }
} 
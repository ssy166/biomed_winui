using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using biomed.Models;
using biomed.Services;

namespace biomed.ViewModels
{
    public partial class ResearchViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        private string _searchQuery;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage;

        public ObservableCollection<ResearchPaper> SearchResults { get; } = new();

        public ResearchViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            SearchResults.Clear();

            try
            {
                var endpoint = $"/research/papers?keyword={Uri.EscapeDataString(SearchQuery)}";
                var results = await _apiClient.GetAsync<List<ResearchPaper>>(endpoint);
                
                if (results != null)
                {
                    foreach (var paper in results)
                    {
                        SearchResults.Add(paper);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"检索出错: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
} 
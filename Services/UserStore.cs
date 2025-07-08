using System.ComponentModel;
using System.Threading.Tasks;
using biomed.Models;
using System;

namespace biomed.Services
{
    public class UserStore : IUserStore
    {
        private readonly ApiClient _apiClient;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            private set { if (_currentUser != value) { _currentUser = value; OnPropertyChanged(nameof(CurrentUser)); OnPropertyChanged(nameof(IsLoggedIn)); } }
        }

        private string _authToken;
        public string AuthToken
        {
            get => _authToken;
            private set { if (_authToken != value) { _authToken = value; OnPropertyChanged(nameof(AuthToken)); } }
        }

        public bool IsLoggedIn => CurrentUser != null;

        public UserStore(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _apiClient.LoginAsync(loginRequest);
            CurrentUser = user;
            // The token is now set within the ApiClient's LoginAsync method
        }

        public async Task RegisterAsync(RegisterRequestDto registerRequest)
        {
            await _apiClient.RegisterAsync(registerRequest);
        }

        public void Logout()
        {
            CurrentUser = null;
            _apiClient.SetAuthToken(null); // Clear the token in ApiClient
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 
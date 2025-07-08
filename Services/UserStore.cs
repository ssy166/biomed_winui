using System.ComponentModel;
using biomed.Models;

namespace biomed.Services
{
    public class UserStore : IUserStore
    {
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

        public void Login(User user, string token)
        {
            CurrentUser = user;
            AuthToken = token;
        }

        public void Logout()
        {
            CurrentUser = null;
            AuthToken = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 
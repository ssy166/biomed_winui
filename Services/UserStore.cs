using System.ComponentModel;
using System.Threading.Tasks;
using biomed.Models;
using System;
using Microsoft.UI.Xaml.Media.Imaging;

namespace biomed.Services
{
    public class UserStore : INotifyPropertyChanged, IUserStore
    {
        private readonly ApiClient _apiClient;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            private set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged(nameof(CurrentUser));
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        private string _authToken;
        public string AuthToken
        {
            get => _authToken;
            private set { if (_authToken != value) { _authToken = value; OnPropertyChanged(nameof(AuthToken)); } }
        }

        private string _csrfToken;
        public string CsrfToken
        {
            get => _csrfToken;
            private set { if (_csrfToken != value) { _csrfToken = value; OnPropertyChanged(nameof(CsrfToken)); } }
        }

        public bool IsLoggedIn => CurrentUser != null;

        public UserStore(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task LoginAsync(LoginRequestDto loginRequest)
        {
            try 
            {
                var user = await _apiClient.LoginWithoutCsrfAsync(loginRequest);
                
                if (user != null)
                {
                    // Set the profile image safely
                    Uri imageUri;
                    if (!string.IsNullOrEmpty(user.AvatarUrl) && Uri.TryCreate(user.AvatarUrl, UriKind.Absolute, out imageUri))
                    {
                        try 
                        {
                            user.ProfileImage = new BitmapImage(imageUri);
                        }
                        catch (Exception)
                        {
                            user.ProfileImage = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
                        }
                    }
                    else
                    {
                        user.ProfileImage = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
                    }

                    SetCurrentUser(user, user?.Token, user?.CsrfToken);
                }
                else 
                {
                    throw new Exception("登录失败：未返回用户数据");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DiagnoseConnectionAsync()
        {
            return await _apiClient.DiagnoseConnectionAsync();
        }

        public async Task RegisterAsync(RegisterRequestDto registerRequest)
        {
            await _apiClient.RegisterAsync(registerRequest);
        }

        public void Logout()
        {
            SetCurrentUser(null, null, null);
            _apiClient.Logout();
        }

        public void SetCurrentUser(User user, string authToken, string csrfToken)
        {
            CurrentUser = user;
            AuthToken = authToken;
            CsrfToken = csrfToken;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using biomed.Models;
using biomed.Services;
using biomed.Views;

namespace biomed.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        private readonly IUserStore _userStore;
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _username;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _password;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isLoggingIn;

        public LoginViewModel(ApiClient apiClient, IUserStore userStore, NavigationService navigationService)
        {
            _apiClient = apiClient;
            _userStore = userStore;
            _navigationService = navigationService;
        }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !IsLoggingIn;

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync()
        {
            IsLoggingIn = true;
            ErrorMessage = string.Empty;
            try
            {
                var loginRequest = new LoginRequestDto
                {
                    Username = this.Username,
                    Password = this.Password
                };

                await _userStore.LoginAsync(loginRequest);

                // After successful login, UserStore.IsLoggedIn will be true.
                // The UI should react to changes in UserStore.
                // We can navigate away if login is successful.
                if (_userStore.IsLoggedIn)
                {
                    _navigationService.NavigateTo(typeof(HomePage));
                }
                else
                {
                    ErrorMessage = "登录失败: 用户名或密码无效。";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"登录出错: {ex.Message}";
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
    }
} 
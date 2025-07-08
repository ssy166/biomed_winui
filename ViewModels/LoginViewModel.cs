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
                var response = await _apiClient.PostAsync<object, LoginResponse>("/auth/login", new { Username, Password });
                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    _apiClient.SetAuthToken(response.Token);
                    _userStore.Login(response.UserInfo, response.Token);
                    _navigationService.NavigateTo(typeof(HomePage));
                }
                else
                {
                    ErrorMessage = "登录失败: 无效的响应。";
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
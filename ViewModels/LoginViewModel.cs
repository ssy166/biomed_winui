using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using biomed.Models;
using biomed.Services;
using biomed.Views;
using System.Windows.Input;

namespace biomed.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        private readonly IUserStore _userStore;
        private readonly INavigationService _navigationService;

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

        [ObservableProperty]
        private string _diagnosisResult;

        [ObservableProperty]
        private bool _isDiagnosing;

        public LoginViewModel(ApiClient apiClient, IUserStore userStore, INavigationService navigationService)
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
            DiagnosisResult = string.Empty;
            
            try
            {
                // 使用统一的登录方法
                var loginRequest = new LoginRequestDto { Username = Username, Password = Password };
                await _userStore.LoginAsync(loginRequest);
                
                // 登录成功，导航到主页
                _navigationService.Navigate(typeof(HomePage));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"登录失败: {ex.Message}";
                
                // 如果是网络连接错误，自动运行诊断
                if (ex.Message.Contains("404") || ex.Message.Contains("连接") || ex.Message.Contains("网络"))
                {
                    await RunDiagnosisAsync();
                }
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        [RelayCommand]
        private async Task RunDiagnosisAsync()
        {
            IsDiagnosing = true;
            try
            {
                DiagnosisResult = await _userStore.DiagnoseConnectionAsync();
            }
            catch (Exception ex)
            {
                DiagnosisResult = $"诊断失败: {ex.Message}";
            }
            finally
            {
                IsDiagnosing = false;
            }
        }
    }
} 
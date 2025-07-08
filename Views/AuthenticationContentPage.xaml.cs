using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using biomed.Models;
using biomed.Services;
using Microsoft.Extensions.DependencyInjection;

namespace biomed.Views
{
    public sealed partial class AuthenticationContentPage : Page
    {
        public event EventHandler<bool> RequestClose;
        private readonly IUserStore _userStore;
        public event EventHandler LoginSuccess;

        public string Username => UsernameTextBox.Text;
        public string Password => PasswordBox.Password;

        public AuthenticationContentPage()
        {
            this.InitializeComponent();
            _userStore = App.Services.GetRequiredService<IUserStore>();
            PrimaryButton.Click += OnLoginClicked;
            CloseButton.Click += (s, e) => RequestClose?.Invoke(this, false);
        }

        private async void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginRequest = new LoginRequestDto { Username = Username, Password = Password };
                await _userStore.LoginAsync(loginRequest);
                LoginSuccess?.Invoke(this, EventArgs.Empty);
                RequestClose?.Invoke(this, true); // Close dialog with success result
            }
            catch (Exception ex)
            {
                // Find the InfoBar and show the error.
                if (this.FindName("InfoBar") is InfoBar infoBar)
                {
                    infoBar.Title = "登录失败";
                    infoBar.Message = ex.Message;
                    infoBar.Severity = InfoBarSeverity.Error;
                    infoBar.IsOpen = true;
                }
            }
        }

        private async void OnRegisterClicked(object sender, RoutedEventArgs e)
        {
            var regUsername = RegisterUsernameTextBox.Text;
            var regPassword = RegisterPasswordBox.Password;
            var confirmPassword = RegisterConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(regUsername) || string.IsNullOrWhiteSpace(regPassword))
            {
                ShowInfoBar("错误", "用户名和密码不能为空。", InfoBarSeverity.Error);
                return;
            }

            if (regPassword != confirmPassword)
            {
                ShowInfoBar("错误", "两次输入的密码不匹配。", InfoBarSeverity.Error);
                return;
            }

            try
            {
                var registerRequest = new RegisterRequestDto
                {
                    Username = regUsername,
                    Password = regPassword
                };
                await _userStore.RegisterAsync(registerRequest);

                ShowInfoBar("成功", "注册成功！现在您可以使用新账户登录了。", InfoBarSeverity.Success);
            }
            catch (Exception ex)
            {
                ShowInfoBar("注册失败", ex.Message, InfoBarSeverity.Error);
            }
        }

        private void ShowInfoBar(string title, string message, InfoBarSeverity severity)
        {
            InfoBar.Title = title;
            InfoBar.Message = message;
            InfoBar.Severity = severity;
            InfoBar.IsOpen = true;
        }

        // Expose buttons for ContentDialog
        public Button GetPrimaryButton() => PrimaryButton;
        public Button GetCloseButton() => CloseButton;
    }
} 
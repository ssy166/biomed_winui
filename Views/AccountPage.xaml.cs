using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using biomed.ViewModels;
using System;

namespace biomed.Views
{
    public sealed partial class AccountPage : Page
    {
        public AccountViewModel ViewModel { get; }

        public AccountPage()
        {
            ViewModel = App.GetService<AccountViewModel>();
            this.InitializeComponent();
        }

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建密码输入框
            var oldPasswordBox = new PasswordBox
            {
                Header = "当前密码",
                PlaceholderText = "请输入当前密码"
            };

            var newPasswordBox = new PasswordBox
            {
                Header = "新密码",
                PlaceholderText = "请输入新密码"
            };

            var confirmPasswordBox = new PasswordBox
            {
                Header = "确认新密码",
                PlaceholderText = "请再次输入新密码"
            };

            // 创建对话框内容
            var content = new StackPanel
            {
                Spacing = 16,
                Width = 320
            };
            content.Children.Add(oldPasswordBox);
            content.Children.Add(newPasswordBox);
            content.Children.Add(confirmPasswordBox);

            // 创建对话框
            var dialog = new ContentDialog
            {
                Title = "修改密码",
                Content = content,
                PrimaryButtonText = "确认修改",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            // 显示对话框并处理结果
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await HandlePasswordChange(oldPasswordBox.Password, newPasswordBox.Password, confirmPasswordBox.Password);
            }
        }

        private async System.Threading.Tasks.Task HandlePasswordChange(string oldPassword, string newPassword, string confirmPassword)
        {
            // 简单验证
            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                await ShowMessageDialog("错误", "请输入当前密码");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                await ShowMessageDialog("错误", "请输入新密码");
                return;
            }

            if (newPassword != confirmPassword)
            {
                await ShowMessageDialog("错误", "两次输入的新密码不一致");
                return;
            }

            if (newPassword.Length < 6)
            {
                await ShowMessageDialog("错误", "新密码长度至少为6位");
                return;
            }

            try
            {
                // 调用ViewModel的修改密码命令
                if (ViewModel.UpdatePasswordCommand.CanExecute(oldPassword))
                {
                    ViewModel.UpdatePasswordCommand.Execute(oldPassword);
                    await ShowMessageDialog("成功", "密码修改成功！");
                }
            }
            catch (Exception ex)
            {
                await ShowMessageDialog("错误", $"密码修改失败：{ex.Message}");
            }
        }

        private async void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowMessageDialog("功能开发中", "个人资料编辑功能正在开发中，敬请期待！");
        }

        private async void BackupDataButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowMessageDialog("功能开发中", "数据备份功能正在开发中，敬请期待！");
        }

        private async void NotificationSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowMessageDialog("功能开发中", "通知设置功能正在开发中，敬请期待！");
        }

        private async void PrivacySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowMessageDialog("功能开发中", "隐私设置功能正在开发中，敬请期待！");
        }

        private async System.Threading.Tasks.Task ShowMessageDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
} 
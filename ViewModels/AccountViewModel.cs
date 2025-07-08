using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using biomed.Services;
using System;
using System.Threading.Tasks;

namespace biomed.ViewModels
{
    public partial class AccountViewModel : ObservableObject
    {
        public IUserStore UserStore { get; }
        private readonly ApiClient _apiClient;

        public AccountViewModel(IUserStore userStore, ApiClient apiClient)
        {
            UserStore = userStore;
            _apiClient = apiClient;
        }

        [RelayCommand]
        private void Logout()
        {
            UserStore.Logout();
            // Navigation back to home or login page should be handled by the Shell
        }

        [RelayCommand]
        private async Task UpdatePassword(object passwords)
        {
            // This is a simplified implementation. 
            // In a real app, you'd want to get all 3 passwords securely.
            // This example just shows the flow.
            try
            {
                // You would need to create a proper DTO and get all password values
                // from the UI. The parameter passing in XAML is just an example.
                // var passwordData = new { old_pwd = "...", new_pwd = "...", re_pwd = "..." };
                // await _apiClient.UpdatePasswordAsync(passwordData);
                // Show success message
            }
            catch (Exception)
            {
                // Show error message
            }
        }
    }
} 
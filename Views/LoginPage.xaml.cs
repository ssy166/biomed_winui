using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using biomed.ViewModels;

namespace biomed.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; }

        public LoginPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<LoginViewModel>();
        }
    }
} 
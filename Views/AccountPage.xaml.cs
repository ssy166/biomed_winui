using Microsoft.UI.Xaml.Controls;
using biomed.ViewModels;

namespace biomed.Views
{
    public sealed partial class AccountPage : Page
    {
        public AccountViewModel ViewModel { get; }

        public AccountPage()
        {
            this.InitializeComponent();
            // ViewModel will be injected by the NavigationService or DI framework
            ViewModel = App.GetService<AccountViewModel>();
        }
    }
} 
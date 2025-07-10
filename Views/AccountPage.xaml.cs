using Microsoft.UI.Xaml.Controls;
using biomed.ViewModels;

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
    }
} 
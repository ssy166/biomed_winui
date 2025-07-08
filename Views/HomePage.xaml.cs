using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using biomed.ViewModels;

namespace biomed.Views
{
    public sealed partial class HomePage : Page
    {
        public HomeViewModel ViewModel { get; }

        public HomePage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<HomeViewModel>();
        }
    }
} 
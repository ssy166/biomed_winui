using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using biomed.ViewModels;

namespace biomed.Views
{
    public sealed partial class ResearchPage : Page
    {
        public ResearchViewModel ViewModel { get; }

        public ResearchPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ResearchViewModel>();
        }
    }
} 
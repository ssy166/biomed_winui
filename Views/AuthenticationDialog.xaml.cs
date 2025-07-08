using Microsoft.UI.Xaml.Controls;

namespace biomed.Views
{
    public sealed partial class AuthenticationDialog : ContentDialog
    {
        public AuthenticationDialog(Page content)
        {
            this.InitializeComponent();
            this.Content = content;
        }
    }
} 
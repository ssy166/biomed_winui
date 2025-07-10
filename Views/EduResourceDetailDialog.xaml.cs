using biomed.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace biomed.Views
{
    public sealed partial class EduResourceDetailDialog : UserControl
    {
        public EduResourceDetail Detail { get; }
        public string FormattedCreatedAt => Detail.CreatedAt.ToString("yyyy-MM-dd");

        public EduResourceDetailDialog(EduResourceDetail detail)
        {
            this.InitializeComponent();
            Detail = detail;
            this.Loaded += (sender, args) =>
            {
                // Fire and forget to avoid deadlocking the UI thread
                _ = InitializeWebViewAsync();
            };
        }

        private async Task InitializeWebViewAsync()
        {
            await ContentView.EnsureCoreWebView2Async();
            ContentView.NavigateToString(Detail.Content ?? string.Empty);
        }

        public void Cleanup()
        {
            ContentView.Close();
        }
    }
} 
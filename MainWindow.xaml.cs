using Microsoft.UI.Xaml;
using biomed.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace biomed
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "中医药信息平台";
            ExtendsContentIntoTitleBar = true; // Enable custom title bar support
            RootFrame.Navigate(typeof(ShellPage));
        }

        public void SetTitleBar(UIElement element)
        {
            // This is a pass-through to the base Window method
            base.SetTitleBar(element);
        }
    }
}

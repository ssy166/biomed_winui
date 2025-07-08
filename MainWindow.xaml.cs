using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using biomed.Services;
using biomed.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace biomed
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window, INotifyPropertyChanged
    {
        public IUserStore UserStore { get; }
        public bool IsLoggedIn => UserStore.IsLoggedIn;
        public bool IsLoggedOut => !UserStore.IsLoggedIn;

        public MainWindow()
        {
            this.InitializeComponent();
            UserStore = App.GetService<IUserStore>();
            UserStore.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(IUserStore.IsLoggedIn))
                {
                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(IsLoggedOut));
                }
            };
            // Initial navigation
            NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First();
            ContentFrame.Navigate(typeof(HomePage));
            Title = "中医药信息平台";
            ExtendsContentIntoTitleBar = true; // Enable custom title bar support
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Navigate to settings page
                return;
            }

            // Regular menu item invoked
            if (args.InvokedItemContainer?.Tag is string pageTag)
            {
                var pageType = Type.GetType(pageTag);
                 if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
                {
                    ContentFrame.Navigate(pageType);
                }
            }
        }
        
        private async void UserAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserStore.IsLoggedIn)
            {
                // If logged in, navigate to Account Page
                ContentFrame.Navigate(typeof(AccountPage));
                // Also select the "My Account" item in nav view if it exists
                NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => x.Tag as string == "biomed.Views.AccountPage") 
                                    ?? NavView.FooterMenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => x.Tag as string == "biomed.Views.AccountPage");
            }
            else
            {
                // If not logged in, show the custom authentication dialog
                var contentPage = new AuthenticationContentPage();
                var dialog = new AuthenticationDialog(contentPage)
                {
                    XamlRoot = this.Content.XamlRoot,
                };

                contentPage.RequestClose += (s, success) => 
                {
                    dialog.Hide();
                };

                await dialog.ShowAsync();
            }
        }
    }
}

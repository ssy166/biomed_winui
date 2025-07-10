using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using biomed.ViewModels;
using biomed.Services;

namespace biomed.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; }
        private readonly INavigationService _navigationService;

        private readonly Dictionary<string, Type> _pages = new()
        {
            { "home", typeof(HomePage) },
            { "formula", typeof(FormulaPage) },
            { "research_platform", typeof(ResearchPlatformPage) },
            { "education", typeof(EducationPage) },
            { "login", typeof(LoginPage) },
            { "account", typeof(AccountPage) }
        };

        public ShellPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ShellViewModel>();
            _navigationService = App.Services.GetRequiredService<INavigationService>();
            
            this.Loaded += OnShellLoaded;
            ContentFrame.Navigated += OnFrameNavigated;

            // Subscribe to the Tapped event for the footer item
            LoginLogoutItem.Tapped += LoginLogoutItem_Tapped;
        }

        private void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            var window = App.Services.GetRequiredService<MainWindow>();
            window.SetTitleBar(AppTitleBar);
            
            if (_navigationService is NavigationService concreteNavigationService)
            {
                concreteNavigationService.AppFrame = this.ContentFrame;
            }

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            
            UpdateLoginLogoutButton();
            _navigationService.NavigateTo(_pages["home"]);
        }
        
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.User) || 
                e.PropertyName == nameof(ViewModel.IsUserLoggedIn) || 
                e.PropertyName == nameof(ViewModel.UserDisplayName))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateLoginLogoutButton();
                });
            }
        }

        private void UpdateLoginLogoutButton()
        {
            if (ViewModel.User != null)
            {
                // 简化显示文本，避免文本过长
                LoginLogoutItem.Content = "注销";
                LoginLogoutItem.Icon = new FontIcon { Glyph = "\uE7E8" };
                LoginLogoutItem.Tag = "logout_action";
                
                // 设置工具提示显示完整信息
                var username = ViewModel.User.Username;
                ToolTipService.SetToolTip(LoginLogoutItem, $"注销 {username}");
            }
            else
            {
                LoginLogoutItem.Content = "登录";
                LoginLogoutItem.Icon = new FontIcon { Glyph = "\uE8D4" };
                LoginLogoutItem.Tag = "login";
                ToolTipService.SetToolTip(LoginLogoutItem, "登录到您的账户");
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // IMPORTANT: Ignore the footer item in this handler, as it's handled by Tapped event.
            if (args.InvokedItemContainer == LoginLogoutItem)
            {
                return;
            }

            if (args.InvokedItemContainer?.Tag is string tag)
            {
                if (_pages.TryGetValue(tag, out var pageType))
                {
                    _navigationService.NavigateTo(pageType);
                }
            }
        }

        private async void LoginLogoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is NavigationViewItem item && item.Tag is string tag)
            {
                if (tag == "logout_action")
                {
                    // Directly call the UserStore's Logout method
                    var userStore = App.Services.GetRequiredService<IUserStore>();
                    userStore.Logout();
                    _navigationService.NavigateTo(_pages["home"]);
                }
                else if (tag == "login")
                {
                    var contentPage = App.GetService<AuthenticationContentPage>();
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

        private void UserAvatar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.User != null)
            {
                _navigationService.NavigateTo(typeof(AccountPage));
            }
        }
        
        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            var pageType = e.SourcePageType;
            var tag = _pages.FirstOrDefault(p => p.Value == pageType).Key;

            if (tag == "login")
            {
                NavView.SelectedItem = null;
                return;
            }

            var activeItem = NavView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag is string itemTag && itemTag == tag);

            NavView.SelectedItem = activeItem ?? NavView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();
        }
    }
} 
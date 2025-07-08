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
        private readonly NavigationService _navigationService;

        private readonly Dictionary<string, Type> _pages = new()
        {
            { "home", typeof(HomePage) },
            { "formula", typeof(FormulaPage) },
            { "research", typeof(ResearchPage) },
            { "login", typeof(LoginPage) }
        };

        public ShellPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ShellViewModel>();
            _navigationService = App.Services.GetRequiredService<NavigationService>();
            
            this.Loaded += OnShellLoaded;
            ContentFrame.Navigated += OnFrameNavigated;

            // Subscribe to the Tapped event for the footer item
            LoginLogoutItem.Tapped += LoginLogoutItem_Tapped;
        }

        private void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            var window = App.Services.GetRequiredService<MainWindow>();
            window.SetTitleBar(AppTitleBar);
            
            _navigationService.AppFrame = this.ContentFrame;

            ViewModel.UserStore.PropertyChanged += OnLoginStateChanged;
            
            UpdateLoginLogoutButton();
            _navigationService.NavigateTo(_pages["home"]);
        }
        
        private void OnLoginStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.UserStore.IsLoggedIn))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateLoginLogoutButton();
                });
            }
        }

        private void UpdateLoginLogoutButton()
        {
            if (ViewModel.UserStore.IsLoggedIn)
            {
                LoginLogoutItem.Content = $"注销 ({ViewModel.UserStore.CurrentUser.Username})";
                LoginLogoutItem.Icon = new FontIcon { Glyph = "\uE7E8" };
                LoginLogoutItem.Tag = "logout_action";
            }
            else
            {
                LoginLogoutItem.Content = "登录";
                LoginLogoutItem.Icon = new FontIcon { Glyph = "\uE8D4" };
                LoginLogoutItem.Tag = "login";
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

        private void LoginLogoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is NavigationViewItem item && item.Tag is string tag)
            {
                if (tag == "logout_action")
                {
                    ViewModel.UserStore.Logout();
                    _navigationService.NavigateTo(_pages["home"]);
                }
                else if (tag == "login")
                {
                    _navigationService.NavigateTo(_pages["login"]);
                }
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
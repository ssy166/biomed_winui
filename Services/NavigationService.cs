using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace biomed.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _appFrame;
        public Frame AppFrame
        {
            get => _appFrame;
            set
            {
                if (_appFrame != null)
                {
                    _appFrame.Navigated -= OnFrameNavigated;
                }
                _appFrame = value;
                if (_appFrame != null)
                {
                    _appFrame.Navigated += OnFrameNavigated;
                }
            }
        }

        public event NavigatedEventHandler Navigated;

        public bool CanGoBack => AppFrame?.CanGoBack ?? false;

        public bool GoBack()
        {
            if (CanGoBack)
            {
                AppFrame.GoBack();
                return true;
            }
            return false;
        }
        
        public void NavigateTo(Type page)
        {
            Navigate(page, null);
        }

        public void Navigate(Type page, object parameter = null)
        {
            if (AppFrame != null && AppFrame.CurrentSourcePageType != page)
            {
                AppFrame.Navigate(page, parameter);
            }
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            Navigated?.Invoke(sender, e);
        }
    }
} 
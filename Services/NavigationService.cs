using Microsoft.UI.Xaml.Controls;
using System;

namespace biomed.Services
{
    public class NavigationService : INavigationService
    {
        public Frame AppFrame { get; set; }

        public void NavigateTo(Type pageType)
        {
            if (AppFrame != null && AppFrame.CurrentSourcePageType != pageType)
            {
                AppFrame.Navigate(pageType);
            }
        }
    }
} 
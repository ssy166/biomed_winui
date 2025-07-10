using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml.Navigation;

namespace biomed.Services
{
    public interface INavigationService
    {
        event NavigatedEventHandler Navigated;
        
        bool CanGoBack { get; }
        
        void NavigateTo(Type page);
        
        void Navigate(Type page, object parameter = null);

        bool GoBack();
    }
} 
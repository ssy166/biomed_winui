using Microsoft.UI.Xaml.Controls;
using System;

namespace biomed.Services
{
    public interface INavigationService
    {
        Frame AppFrame { get; set; }
        void NavigateTo(Type pageType);
    }
} 
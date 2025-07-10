using CommunityToolkit.Mvvm.ComponentModel;
using biomed.Services;
using System.ComponentModel;
using biomed.Models;
using Microsoft.UI.Xaml.Media.Imaging;

namespace biomed.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        private readonly IUserStore _userStore;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _isPaneOpen = true;
        
        [ObservableProperty]
        private User _user;

        // 安全的计算属性，避免绑定时空引用
        public BitmapImage UserProfileImage => User?.ProfileImage;
        public string UserDisplayName => User?.Username ?? "游客";
        public bool IsUserLoggedIn => User != null;

        public ShellViewModel(INavigationService navigationService, IUserStore userStore)
        {
            _userStore = userStore;
            _user = _userStore.CurrentUser;

            if (_userStore is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged += OnUserStorePropertyChanged;
            }
            
            _navigationService = navigationService;
            _navigationService.Navigated += OnNavigated;
        }

        private void OnUserStorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserStore.CurrentUser))
            {
                User = _userStore.CurrentUser;
            }
        }

        partial void OnUserChanged(User value)
        {
            // 当用户改变时，通知相关计算属性也发生了变化
            OnPropertyChanged(nameof(UserProfileImage));
            OnPropertyChanged(nameof(UserDisplayName));
            OnPropertyChanged(nameof(IsUserLoggedIn));
        }

        private void OnNavigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // Update UI based on navigation, if needed. For example, selected item.
        }
    }
} 
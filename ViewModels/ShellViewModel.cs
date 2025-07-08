using CommunityToolkit.Mvvm.ComponentModel;
using biomed.Services;

namespace biomed.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isPaneOpen = true;

        public IUserStore UserStore { get; }

        public ShellViewModel(IUserStore userStore)
        {
            UserStore = userStore;
        }
    }
} 
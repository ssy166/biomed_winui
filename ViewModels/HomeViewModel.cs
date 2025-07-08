using CommunityToolkit.Mvvm.ComponentModel;
using biomed.Services;

namespace biomed.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        public IUserStore UserStore { get; }

        public HomeViewModel(IUserStore userStore)
        {
            UserStore = userStore;
        }
    }
} 
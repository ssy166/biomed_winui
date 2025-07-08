using System.ComponentModel;
using biomed.Models;

namespace biomed.Services
{
    public interface IUserStore : INotifyPropertyChanged
    {
        User CurrentUser { get; }
        string AuthToken { get; }
        bool IsLoggedIn { get; }
        void Login(User user, string token);
        void Logout();
    }
} 
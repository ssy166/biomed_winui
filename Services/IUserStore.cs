using System.ComponentModel;
using System.Threading.Tasks;
using biomed.Models;

namespace biomed.Services
{
    public interface IUserStore : INotifyPropertyChanged
    {
        User CurrentUser { get; }
        bool IsLoggedIn { get; }
        Task LoginAsync(LoginRequestDto loginRequest);
        Task RegisterAsync(RegisterRequestDto registerRequest);
        void Logout();
    }
} 
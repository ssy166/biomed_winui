using System.ComponentModel;
using System.Threading.Tasks;
using biomed.Models;

namespace biomed.Services
{
    public interface IUserStore
    {
        User CurrentUser { get; }
        string AuthToken { get; }
        string CsrfToken { get; }
        bool IsLoggedIn { get; }

        event PropertyChangedEventHandler PropertyChanged;

        Task LoginAsync(LoginRequestDto loginRequest);
        Task RegisterAsync(RegisterRequestDto registerRequest);
        void Logout();
        void SetCurrentUser(User user, string authToken, string csrfToken);
        Task<string> DiagnoseConnectionAsync();
    }
} 
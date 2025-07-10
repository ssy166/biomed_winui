using Microsoft.UI.Xaml.Media.Imaging;

namespace biomed.Models
{
    public class User
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public int Role { get; set; } // 0-admin, 1-student, 2-teacher
        public string Email { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        
        // Token and CSRF token are part of the user session data
        public string Token { get; set; }
        public string CsrfToken { get; set; }

        // This property is for UI binding
        public BitmapImage ProfileImage { get; set; }
    }
} 
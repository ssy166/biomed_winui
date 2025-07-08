namespace biomed.Models
{
    // For /api/users/login response
    public class LoginToken
    {
        public string Token { get; set; }
    }

    // For /api/users/userInfo response
    public class User
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        
        // This property will be populated after login, it's not part of the userInfo response.
        public string Token { get; set; }
    }
} 
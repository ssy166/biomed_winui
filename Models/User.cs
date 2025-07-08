namespace biomed.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public User UserInfo { get; set; }
    }
} 
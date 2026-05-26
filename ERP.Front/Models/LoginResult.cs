namespace ERP.Front.Models
{
    public class LoginResult
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
        public UserData User { get; set; } = new();
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
       // public UserData? User { get; private set; }

        public static LoginResult Success(UserData user) => new()
        {
            IsSuccess = true,
            User = user
        };

        public static LoginResult Failure(string message) => new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };

    }
}

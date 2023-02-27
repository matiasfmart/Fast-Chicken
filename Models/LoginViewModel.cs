namespace FastChicken.Models
{
    public class LoginViewModel
    {
        public string? User { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        public string? errorMessage { get; set; }
    }
}

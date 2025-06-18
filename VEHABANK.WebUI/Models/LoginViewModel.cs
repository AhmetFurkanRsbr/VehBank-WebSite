namespace VEHABANK.WebUI.Models
{
    public class LoginViewModel
    {
        public string? CustomerNumberOrIdentityNumber { get; set; }
        public string Password { get; set; } = null!;
    }
}

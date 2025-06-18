namespace VEHABANK.Shared.Dto
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } // JWT veya session verisi
        public string Message { get; set; }
    }
}

namespace VEHABANK.Shared.Dto
{
    public class LoginDto
    {
        public string Identifier { get; set; } //Müşteri numarası veya kimlik numarası(customerNumber-identityNumber)
        public string Password { get; set; }

    }
}

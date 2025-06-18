namespace VEHABANK.Shared.Dto
{
    public class RegisterDto
    {
        public string BranchName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        // Kullanıcıdan tarih string olarak alınacak
        public string BirthDate { get; set; }
        public string IdentityNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        //public string CustomerNumber { get; set; }
        public string Password { get; set; }
        public string AuthenticateQuestion { get; set; }
        public string AuthenticateAnswerForQuestion { get; set; }
    }
}

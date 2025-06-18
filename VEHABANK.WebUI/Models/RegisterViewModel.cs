using System.ComponentModel.DataAnnotations;

namespace VEHABANK.WebUI.Models
{
    public class RegisterViewModel
    {
       
        [Required] public string BranchName { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        public string BirthDate { get; set; } // "dd-MM-yyyy" formatında gönderilecek
        [Required, StringLength(11)] public string IdentityNumber { get; set; }
        public string Email { get; set; }
        [Required, StringLength(10)] public string Phone { get; set; }
        [Required] public string CustomerNumber { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [Required] public string AuthenticateAnswerForQuestion { get; set; }
        [Required] public int AuthenticateQuestion { get; set; }
    }
}

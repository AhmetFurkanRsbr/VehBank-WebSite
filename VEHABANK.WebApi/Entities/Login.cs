using System.ComponentModel.DataAnnotations;
using VEHABANK.WebApi.Enums;

namespace VEHABANK.WebApi.Entities
{
    public class Login
    {
       
        [Key]
        [MaxLength(5)]
        public string CustomerNumber { get; set; }
        public Authorization Authorization  { get; set; }
        public string PasswordHash { get; set; }
        public AuthenticateQuestion AuthenticateQuestion { get; set; }
        public string AuthenticateAnswerForQuestion { get; set; }
        public User User { get; set; }  // Navigation Property

    }
}

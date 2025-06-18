using System.ComponentModel.DataAnnotations;
using VEHABANK.WebApi.Enums;

namespace VEHABANK.WebApi.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }//İkincil Anahtar - İlişkili User tablosu
        public User User { get; set; }// Navigation property

        public int BranchId { get; set; } //İkincil Anahtar - İlişkili Branch tablosu
        public Branch Branch { get; set; }// Navigation property

        public string AccountNumber { get; set; }
        [Required]
        [StringLength(26, MinimumLength = 26, ErrorMessage = "IBAN 26 karakter olmalıdır.")]
        public string IBAN { get; set; }
        public Decimal Balance { get; set; } //Bakiye
        public Currency Currency { get; set; } //Para birimi
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
      
    }
}

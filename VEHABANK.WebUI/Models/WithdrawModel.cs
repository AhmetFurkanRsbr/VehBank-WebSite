using System.ComponentModel.DataAnnotations;

namespace VEHABANK.WebUI.Models
{
    public class WithdrawModel
    {
        [Required(ErrorMessage = "Hesap numarası zorunludur.")]
        [Display(Name = "Hesap Numarası")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır.")]
        [Display(Name = "Miktar")]
        public decimal Amount { get; set; }

        [Display(Name = "Açıklama (İsteğe Bağlı)")]
        public string? Description { get; set; }
    }
} 
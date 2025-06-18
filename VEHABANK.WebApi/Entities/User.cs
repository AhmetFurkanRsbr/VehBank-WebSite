using System.ComponentModel.DataAnnotations;
using VEHABANK.WebApi.Enums;

namespace VEHABANK.WebApi.Entities
{
    public class User 
    {
        public int Id { get; set; }
        public int BranchId { get; set; } //Fk
        public Branch Branch { get; set; } // Navigation
        public ICollection<Login> Logins { get; set; } // Navigation
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly BirthDate { get; set; }
        // Formatlı doğum tarihi (yyyy-MM-dd formatında döndürülür)
        public string BirthDateFormatted => BirthDate.ToString("yyyy-MM-dd");

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Tc 11 karakter olmalıdır.")]
        public string IdentityNumber { get; set; }//TC kimlik no
        public string Email { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Telefon numarası 10 karakter olmalıdır.")]
        public string Phone { get; set; }
        public bool isActive { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt{ get; set; }
    }
}

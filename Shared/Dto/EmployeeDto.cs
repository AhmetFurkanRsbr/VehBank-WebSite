using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VEHABANK.Shared.Dto
{
    public class EmployeeDto
    {
        public int Id { get; set; } // Güncelleme senaryosu için

        //[Required]
        public string Name { get; set; }

        //[Required]
        public string Surname { get; set; }

        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Telefon numarası 10 karakter olmalıdır.")]
        public string Phone { get; set; }

        //[Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "T.C. kimlik numarası 11 haneli olmalıdır.")]
        public string IdentityNumber { get; set; }

        //[Required]
        public string BirthDate { get; set; } // "yyyy-MM-dd" formatında string olarak alınacak (frontend'den)

        //[Required]
        public int BranchId { get; set; } // Şube ID’si (dropdown'dan alınır)

        public string? BranchName { get; set; }
        //[Required]
        public string CustomerNumber { get; set; } // Generate edilen personel müşteri numarası

        //[Required]
        public int Authorization { get; set; } // Enum’dan seçilir (0=Admin, 1=Yönetici, 2=Gişe Görevlisi)
    }

}

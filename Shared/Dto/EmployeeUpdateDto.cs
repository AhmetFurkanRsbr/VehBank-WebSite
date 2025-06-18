using System.ComponentModel.DataAnnotations;

namespace VEHABANK.Shared.Dto
{
    public class EmployeeUpdateDto
    {
        public int Id { get; set; } // Güncelleme işlemi için gerekli

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int BranchId { get; set; }
        public int Authorization { get; set; }
        public string CustomerNumber { get; set; }


        // Okunabilir alanlar ve değiştirilmeyen veriler bu DTO'ya eklenmez
        // public string Name { get; set; }
        // public string Surname { get; set; }
        // public string CustomerNumber { get; set; }
    }
}

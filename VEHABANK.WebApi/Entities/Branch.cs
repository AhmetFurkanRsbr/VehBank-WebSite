using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VEHABANK.WebApi.Entities
{
    public class Branch //Şubeler
    {
        public int Id { get; set; }

        [Required]
        [StringLength(85, ErrorMessage = "Şube adı en fazla 85 karakter olmalıdır.")]
        public string Name { get; set; } 

        public string Address { get; set; } 

        public string City { get; set; } 

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Telefon numarası 10 karakter olmalıdır.")]
        public string Phone { get; set; }

        public string Email { get; set; } 

        // Foreign Key ilişkisi, bir şube birden fazla kullanıcıya sahip olabilir
        public ICollection<User> Users { get; set; }

    }
}

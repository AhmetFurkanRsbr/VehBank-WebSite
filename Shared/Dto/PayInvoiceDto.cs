using System.ComponentModel.DataAnnotations;

namespace VEHABANK.Shared.DTOs
{
    public class PayInvoiceDto
    {
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        public string Description { get; set; }
    }
} 

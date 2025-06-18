using VEHABANK.WebApi.Enums;

namespace VEHABANK.WebApi.Entities
{
    public class Transfer //Para transfer işlemleri
    {
        public int Id { get; set; }
        public int PerformedByUserId { get; set; } // Foreign key
        public User PerformedByUser { get; set; }// İşlemi kimin gerçekleştirdiği --->İkincil Anahtar - İlişkili User tablosu
        public Decimal Amount { get; set; }  //Miktar
        public Transaction Transaction { get; set; }  //İşlem Türü HAVALE-EFT..
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public string? Description { get; set; }
        public DateTime? TransferDate { get; set; } // Transfer zamanı 
    }
}

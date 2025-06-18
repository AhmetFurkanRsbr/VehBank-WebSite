using VEHABANK.WebApi.Enums;

namespace VEHABANK.WebApi.Entities
{
    public class TransactionHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }// İşlemi kimin gerçekleştirdiği --->İkincil Anahtar - İlişkili User tablosu
        public User User { get; set; }// Navigation 
        public Transaction Transaction  { get; set; } //İşlem Türü
        public DateTime Date{ get; set; }

        public int AccountId { get; set; } // İşlemin yapıldığı hesap
        public Account Account { get; set; } // Navigation property
        public decimal Amount { get; set; } // İşlem tutarı
        public string Description { get; set; } // İşlem açıklaması

    }
}

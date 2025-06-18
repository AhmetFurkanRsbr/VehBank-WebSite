namespace VEHABANK.WebApi.Entities
{
    public class LoginLog 
    {
        public int Id { get; set; }
        public int UserId { get; set; }//İkincil Anahtar - İlişkili User tablosu
        public User User{ get; set; }//Navigation
        public DateTime Date{ get; set; }
        public bool Success{ get; set; }
    }
}

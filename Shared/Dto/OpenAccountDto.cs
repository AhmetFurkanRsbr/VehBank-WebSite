namespace VEHABANK.Shared

{
    public class OpenAccountDto
    {
        public int UserId { get; set; }
        public string AccountNumber { get; set; }
        public int BranchId { get; set; }
        public int Currency { get; set; }
        public string IBAN { get; set; }
        public decimal Balance { get; set; }
    }
}

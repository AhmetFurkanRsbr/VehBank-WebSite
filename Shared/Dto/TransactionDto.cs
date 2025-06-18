using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VEHABANK.Shared.Dto
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string UserFullName { get; set; }
        public int BranchId { get; set; }

    }
}

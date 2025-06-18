using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VEHABANK.Shared.Dto
{
    public class TransferDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string Iban { get; set; }
        public int Transaction { get; set; }
        public int PerformedByUserId { get; set; }
        public decimal Amount { get; set; }
    }
}

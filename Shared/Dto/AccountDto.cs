using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VEHABANK.Shared.Dto
{
    public class AccountDto
    {
        public string IBAN { get; set; }
        public decimal Balance { get; set; }
    }
}

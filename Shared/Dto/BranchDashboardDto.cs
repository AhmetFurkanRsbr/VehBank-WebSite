using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VEHABANK.Shared.Dto
{
    public class BranchDashboardDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int UserCount { get; set; }
        public int EmployeeCount { get; set; }
        public decimal TotalBalance { get; set; }
        
    }
}

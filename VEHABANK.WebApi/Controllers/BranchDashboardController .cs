using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VEHABANK.WebApi.Context;
using VEHABANK.WebApi.Enums;
using VEHABANK.Shared.Dto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data;

namespace VEHABANK.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "BranchManager,SeniorManager")] // JWT role bazlı kontrol
    public class BranchDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BranchDashboardController(AppDbContext context)
        {
            _context = context;
        }
        
        // Kullanıcıdan branch bilgisi al
        [HttpGet("GetBranchStats")]
        public IActionResult GetBranchStats()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { Message = "JWT Token geçersiz veya kullanıcı kimliği alınamadı." });

            bool isSeniorManager = roleClaim == Authorization.SeniorManager.ToString();

            int? branchId = null;
            if (!isSeniorManager)
            {
                branchId = _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.BranchId)
                    .FirstOrDefault();

                if (branchId == 0)
                    return NotFound(new { Message = "Kullanıcının bağlı olduğu şube bulunamadı." });
            }

            var usersQuery = _context.Users.AsQueryable();
            var employeesQuery = _context.Logins.Include(l => l.User).Where(l => l.Authorization == Authorization.BankTeller);
            var accountsQuery = _context.Accounts.AsQueryable();

            if (!isSeniorManager)
            {
                usersQuery = usersQuery.Where(u => u.BranchId == branchId);
                employeesQuery = employeesQuery.Where(l => l.User.BranchId == branchId);
                accountsQuery = accountsQuery.Where(a => a.BranchId == branchId);
            }

            var userCount = usersQuery.Count();
            var employeeCount = employeesQuery.Count();
            var totalBalance = accountsQuery.Sum(a => (decimal?)a.Balance) ?? 0;
            var branchName = isSeniorManager
                ? "Tüm Şubeler"
                : _context.Branches.FirstOrDefault(b => b.Id == branchId)?.Name ?? "Bilinmeyen";

            var dto = new BranchDashboardDto
            {
                BranchId = branchId ?? 0,
                BranchName = branchName,
                UserCount = userCount,
                EmployeeCount = employeeCount,
                TotalBalance = totalBalance
            };

            return Ok(dto);
        }

        [HttpGet("GetBranchDashboard/{userId}")]
        public IActionResult GetBranchDashboard(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound(new { Message = "Kullanıcı bulunamadı." });

            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isSeniorManager = roleClaim == Authorization.SeniorManager.ToString();

            int? branchId = isSeniorManager ? null : user.BranchId;

            var branchesQuery = _context.Branches.AsQueryable();
            var loginsQuery = _context.Logins.AsQueryable();
            var accountsQuery = _context.Accounts.AsQueryable();

           
            if (branchId != null)
            {
                loginsQuery = loginsQuery.Where(l => l.User.BranchId == branchId);
            }
            loginsQuery = loginsQuery.Include(l => l.User);

            var personnelCount = loginsQuery.Count(l => l.Authorization == Authorization.BankTeller);
            var customerCount = loginsQuery.Count(l => l.Authorization == Authorization.Customer);
            var totalBalance = accountsQuery.Sum(a => (decimal?)a.Balance) ?? 0;
            var branchName = isSeniorManager
                ? "Tüm Şubeler"
                : _context.Branches.FirstOrDefault(b => b.Id == branchId)?.Name ?? "Bilinmiyor";

            return Ok(new
            {
                BranchName = branchName,
                PersonnelCount = personnelCount,
                CustomerCount = customerCount,
                TotalBalance = totalBalance
            });
        }

        [HttpGet("GetTodayCustomersByBranch")]
        public IActionResult GetTodayCustomersByBranch()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { Message = "Kullanıcı kimliği alınamadı." });

            bool isSeniorManager = roleClaim == Authorization.SeniorManager.ToString();

            int? branchId = null;
            if (!isSeniorManager)
            {
                branchId = _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.BranchId)
                    .FirstOrDefault();

                if (branchId == 0)
                    return NotFound(new { Message = "Kullanıcının bağlı olduğu şube bulunamadı." });
            }

            var today = DateTime.Today;
            var customersQuery = _context.Logins
                .Include(l => l.User)
                .Where(l => l.Authorization == Authorization.Customer &&
                            l.User.CreatedAt.Date == today);

            if (!isSeniorManager)
                customersQuery = customersQuery.Where(l => l.User.BranchId == branchId);

            var customers = customersQuery
                .Select(l => new
                {
                    FullName = l.User.Name + " " + l.User.Surname,
                    CustomerNumber = l.CustomerNumber
                })
                .ToList();

            return Ok(customers);
        }


        // İşlem Geçmişi kısmı (Transaction Histories)
        [HttpGet("GetBranchTransactions")]
        public IActionResult GetBranchTransactions()
        { 
            // Kullanıcı ID'sini token'dan al
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { Message = "Geçerli kullanıcı bulunamadı." });

            // Yetkiyi al
            if (!Enum.TryParse(roleClaim, out Authorization role))
                return Forbid();

            IQueryable<TransactionDto> query = _context.TransactionHistories
                .Include(t => t.Account)
                .ThenInclude(a => a.User)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.Id,
                    TransactionType = ((Enums.Transaction)t.Transaction).ToString(),
                    AccountId = t.AccountId,
                    UserFullName = t.Account.User.Name + " " + t.Account.User.Surname,
                    Amount = t.Amount,
                    Date = t.Date,
                    Status = t.Description,
                    BranchId = t.Account.BranchId
                });

            if (role == Authorization.BranchManager)
            {
                var branchId = _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.BranchId)
                    .FirstOrDefault();

                if (branchId == 0)
                    return NotFound(new { Message = "Kullanıcının bağlı olduğu şube bulunamadı." });

                query = query.Where(t => t.BranchId == branchId);
            }

            var transactions = query
                .OrderByDescending(t => t.Date)
                .ToList();

            return Ok(transactions);
        }


    }
}

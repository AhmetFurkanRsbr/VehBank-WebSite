using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VEHABANK.Shared.Dto;
using VEHABANK.WebApi.Context;
using VEHABANK.WebApi.Entities;
using VEHABANK.WebApi.Enums;
using VEHABANK.WebApi.Service;

namespace VEHABANK.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }
       
        [Authorize(Roles = "Customer,BankTeller")]
        [HttpPost("DepositMoney")]
        public async Task<IActionResult> DepositMoney([FromBody] TransferDto dto)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountNumber == dto.AccountNumber);
            if (account == null)
                return NotFound(new { message = "Hesap bulunamadı." });

            // Bakiyeyi güncelle
            account.Balance += dto.Amount;

            // İşlemi kaydet
            var transfer = new Transfer
            {
                TransferDate = DateTime.Now,
                Amount = dto.Amount,
                Description = "Para yatırma işlemi",
                Transaction = Transaction.Deposit,
                FromAccountId = null, // yatırma işleminde dışardan geliyor
                ToAccountId = account.Id,
                PerformedByUserId = dto.PerformedByUserId
            };

            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();


            // Eğer işlemi yapan Gişe Memuru ise TransactionHistory'ye kaydet
            var performer = await _context.Logins
                .FirstOrDefaultAsync(l => l.User.Id == dto.PerformedByUserId);

            if (performer != null && performer.Authorization == Authorization.BankTeller)
            {
                var history = new TransactionHistory
                {
                    Transaction = Enums.Transaction.Deposit,
                    Date = DateTime.Now,
                    Amount = dto.Amount,
                    Description = "Para yatırma",
                    AccountId = dto.AccountId,
                    UserId = dto.PerformedByUserId, 
                };

                _context.TransactionHistories.Add(history);
                await _context.SaveChangesAsync();
            }


            return Ok(new { message = "Para yatırma işlemi başarılı.", newBalance = account.Balance });
        }


        [HttpGet("GetAccountsForDeposit")]
        [Authorize(Roles = "Customer,BankTeller")]
        public IActionResult GetAccountsForDeposit()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleClaim))
                return Unauthorized(new { Message = "Rol bilgisi alınamadı." });

            IQueryable<Account> query = _context.Accounts;

            if (roleClaim == Authorization.Customer.ToString())
            {
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { Message = "Kullanıcı kimliği alınamadı." });

                query = query.Where(a => a.UserId == userId);
            }

            var accounts = query.Select(a => new
            {
                a.Id,
                a.AccountNumber,
                a.IBAN,
                a.Balance,
                a.UserId,
                CustomerNumber = a.User.Logins.FirstOrDefault().CustomerNumber.ToString()
            }).ToList();

            return Ok(accounts);
        }

        //---------------------------------------------------transfer
        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Kullanıcı ID alınamadı.");

            var senderAccount = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == dto.AccountNumber);

            if (senderAccount == null)
                return NotFound("Gönderen hesap bulunamadı.");

            if (senderAccount.Balance < dto.Amount)
                return BadRequest("Yetersiz bakiye.");

            var receiverAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.IBAN.ToUpper() == dto.Iban.ToUpper());

            if (receiverAccount == null)
                return NotFound("Alıcı hesap (IBAN) bulunamadı.");

            if (receiverAccount.Id == senderAccount.Id)
                return BadRequest("Kendi hesabınıza transfer yapılamaz.");

            senderAccount.Balance -= dto.Amount;
            receiverAccount.Balance += dto.Amount;

            var transfer = new Transfer
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Transaction = (Transaction)dto.Transaction,
                TransferDate = DateTime.Now,
                FromAccountId = senderAccount.Id,
                ToAccountId = receiverAccount.Id,
                PerformedByUserId = userId
            };

            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();

            // Eğer işlemi yapan Gişe Memuru ise TransactionHistory'ye kaydet
            var performer = await _context.Logins
                .FirstOrDefaultAsync(l => l.User.Id == dto.PerformedByUserId);

            if (performer != null && performer.Authorization == Authorization.BankTeller)
            {
                var history = new TransactionHistory
                {
                    Transaction = (Transaction)dto.Transaction,
                    Date = DateTime.Now,
                    Amount = dto.Amount,
                    Description = "Para transferi",
                    AccountId = senderAccount.Id,
                    UserId = dto.PerformedByUserId,
                };

                _context.TransactionHistories.Add(history);
                await _context.SaveChangesAsync();
            }


            return Ok(new { Message = "Transfer başarıyla gerçekleştirildi." });
        }
    }
}

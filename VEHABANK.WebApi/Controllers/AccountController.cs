
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEHABANK.WebApi.Context;
using VEHABANK.WebApi.Entities;
using VEHABANK.WebApi.Enums;
using System.Security.Claims;
using VEHABANK.Shared.Dto;

namespace VEHABANK.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockAccount([FromBody] AccountNumberRequest request)
        {
            try
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);

                if (account == null)
                    return NotFound("Hesap bulunamadı.");

                if (!account.IsActive)
                    return BadRequest("Hesap zaten blokeli.");

                account.IsActive = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Hesap başarıyla bloke edildi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu.", error = ex.Message });
            }
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAccount([FromBody] AccountNumberRequest request)
        {
            try
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);

                if (account == null)
                    return NotFound("Hesap bulunamadı.");

                if (account.IsActive)
                    return BadRequest("Hesap zaten aktif.");

                account.IsActive = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Hesap başarıyla aktifleştirildi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu.", error = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawMoney([FromBody] WithdrawRequest request)
        {
            _logger.LogInformation($"WithdrawMoney isteği alındı. AccountNumber: {request.AccountNumber}, Amount: {request.Amount}");

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);

            if (account == null)
            {
                _logger.LogWarning($"Hesap bulunamadı. AccountNumber: {request.AccountNumber}");
                return NotFound("Hesap bulunamadı.");
            }

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("Kullanıcı kimliği alınamadı.");
                return Unauthorized();
            }

            if (account.UserId != userId)
            {
                _logger.LogWarning($"Kullanıcı kendi hesabı dışında bir hesaptan işlem yapmaya çalıştı. Account.UserId: {account.UserId}, UserId: {userId}");
                return Forbid("Bu hesaba erişim yetkiniz yok.");
            }

            if (!account.IsActive)
            {
                _logger.LogWarning($"Hesap aktif değil. AccountNumber: {request.AccountNumber}");
                return BadRequest("Hesap aktif değil.");
            }

            if (account.Balance < request.Amount)
            {
                _logger.LogWarning($"Yetersiz bakiye. Mevcut bakiye: {account.Balance}, İstenen tutar: {request.Amount}");
                return BadRequest("Yetersiz bakiye.");
            }

            try
            {
                account.Balance -= request.Amount;
                _logger.LogInformation($"Bakiye güncellendi. Yeni bakiye: {account.Balance}");

                var transaction = new TransactionHistory
                {
                    AccountId = account.Id,
                    UserId = account.UserId,
                    Amount = -request.Amount,
                    Description = string.IsNullOrEmpty(request.Description) ? "Para çekme işlemi" : request.Description,
                    Date = DateTime.Now,
                    Transaction = Transaction.Withdraw
                };

                _context.TransactionHistories.Add(transaction);
                _logger.LogInformation($"TransactionHistory kaydı oluşturuldu. TransactionId: {transaction.Id}");

                await _context.SaveChangesAsync();
                _logger.LogInformation("Değişiklikler veritabanına kaydedildi.");

                return Ok(new { success = true, newBalance = account.Balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Para çekme işlemi sırasında bir hata oluştu.");
                return StatusCode(500, "Bir hata oluştu."+ex);
            }
        }


        [HttpGet("GetAccountsByUserId/{userId}")]
        //[AllowAnonymous]
        public async Task<ActionResult<List<AccountDto>>> GetAccountsByUserId(int userId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId && a.IsActive)
                .Select(a => new AccountDto
                {
                    IBAN = a.IBAN,
                    Balance = a.Balance
                })
                .ToListAsync();

            return Ok(accounts);
        }

    }

    public class AccountNumberRequest
    {
        public string AccountNumber { get; set; }
    }

    public class WithdrawRequest
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}

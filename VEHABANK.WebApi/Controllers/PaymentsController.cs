using Microsoft.AspNetCore.Mvc;
using VEHABANK.WebApi.Context;
using VEHABANK.Shared.Dto;
using VEHABANK.WebApi.Entities;
using VEHABANK.WebApi.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using VEHABANK.Shared.DTOs;

namespace VEHABANK.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(AppDbContext context, ILogger<PaymentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("pay-invoice")]
        public IActionResult PayInvoice([FromBody] PayInvoiceDto dto)
        {
            _logger.LogInformation($"PayInvoice isteği alındı. AccountId: {dto.AccountId}, Amount: {dto.Amount}, Description: {dto.Description}");

            var account = _context.Accounts.FirstOrDefault(a => a.Id == dto.AccountId);
            if (account == null)
            {
                _logger.LogWarning($"Hesap bulunamadı. AccountId: {dto.AccountId}");
                return NotFound("Account not found.");
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

            _logger.LogInformation($"Hesap bulundu. Mevcut bakiye: {account.Balance}");

            if (account.Balance < dto.Amount)
            {
                _logger.LogWarning($"Yetersiz bakiye. Mevcut bakiye: {account.Balance}, İstenen tutar: {dto.Amount}");
                return BadRequest("Insufficient balance.");
            }

            try
            {
                account.Balance -= dto.Amount;
                _logger.LogInformation($"Bakiye güncellendi. Yeni bakiye: {account.Balance}");

                var transaction = new TransactionHistory
                {
                    AccountId = account.Id,
                    UserId = account.UserId,
                    Amount = -dto.Amount,
                    Description = dto.Description ?? "Fatura/Yurt Ödemesi",
                    Date = DateTime.Now,
                    Transaction = Transaction.Payment
                };

                _context.TransactionHistories.Add(transaction);
                _logger.LogInformation($"TransactionHistory kaydı oluşturuldu. TransactionId: {transaction.Id}");

                _context.SaveChanges();
                _logger.LogInformation("Değişiklikler veritabanına kaydedildi.");

                return Ok(new { success = true, newBalance = account.Balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme işlemi sırasında bir hata oluştu.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEHABANK.Shared;
using VEHABANK.WebApi.Context;
using VEHABANK.WebApi.Entities;

namespace VEHABANK.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenAccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OpenAccountController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAccountNumberAndIban")]
        public async Task<IActionResult> GetAccountNumberAndIban()
        {
            var iban = await GenerateUniqueIbanAsync();
            var accountNumber = await GenerateUniqueAccountNumberAsync();
            return Ok(new { iban, accountNumber });
        }

        [HttpPost]
        public async Task<IActionResult> OpenAccount([FromBody] OpenAccountDto dto)
        {

            var account = new Account
            {
                UserId = dto.UserId,
                BranchId = dto.BranchId,
                IBAN = dto.IBAN,
                AccountNumber = dto.AccountNumber,
                Currency = (Enums.Currency)dto.Currency,
                Balance = dto.Balance,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            
            return Ok("Hesap oluşturuldu");
        }
        private async Task<string> GenerateUniqueIbanAsync()
        {
            string iban;
            do
            {
                iban = "TR" + string.Join("", Enumerable.Range(0, 24).Select(_ => Random.Shared.Next(0, 10)));
            }
            while (await _context.Accounts.AnyAsync(a => a.IBAN == iban));
            return iban;
        }

        private async Task<string> GenerateUniqueAccountNumberAsync()
        {
            string accountNumber;
            do
            {
                accountNumber = Random.Shared.Next(10000, 99999).ToString();// 5 haneli
            }
            while (await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber));
            return accountNumber;
        }
    }
}

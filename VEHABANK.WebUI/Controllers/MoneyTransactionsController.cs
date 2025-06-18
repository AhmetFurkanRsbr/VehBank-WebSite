using Microsoft.AspNetCore.Mvc;
using VEHABANK.WebUI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace VEHABANK.WebUI.Controllers
{
    public class MoneyTransactionsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MoneyTransactionsController> _logger;

        public MoneyTransactionsController(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration,
            ILogger<MoneyTransactionsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(WithdrawModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model geçersiz");
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("VEHABANK.WebApi");
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token bulunamadı");
                    return Json(new { success = false, message = "Oturum süresi dolmuş. Lütfen tekrar giriş yapın." });
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                _logger.LogInformation($"Para çekme isteği gönderiliyor. AccountNumber: {model.AccountNumber}, Amount: {model.Amount}");

                
                //var response = await client.PostAsJsonAsync("https://localhost:7060/api/Account/withdraw", model);
                var response = await client.PostAsJsonAsync("https://localhost:7060/api/Account/withdraw", model);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var newBalance = result.GetProperty("newBalance").GetDecimal();
                    
                    _logger.LogInformation($"Para çekme işlemi başarılı. Yeni bakiye: {newBalance}");
                    
                    return Json(new { 
                        success = true, 
                        message = $"Para çekme işlemi başarıyla gerçekleştirildi. Yeni bakiyeniz: {newBalance:C}",
                        newBalance = newBalance
                    });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Para çekme işlemi başarısız. Hata: {error}");
                    return Json(new { success = false, message = $"İşlem başarısız: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Para çekme işlemi sırasında bir hata oluştu");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
    }
} 
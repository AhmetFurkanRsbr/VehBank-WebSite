using System.Net.Http.Json;
using VEHABANK.Shared.DTOs;
using VEHABANK.WebUI.Models;

namespace VEHABANK.WebUI.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(HttpClient httpClient, ILogger<PaymentService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(bool success, decimal? newBalance)> PayInvoice(PayInvoiceDto request)
        {
            try
            {
                _logger.LogInformation($"PayInvoice isteği gönderiliyor. AccountId: {request.AccountId}, Amount: {request.Amount}, Description: {request.Description}");

                var response = await _httpClient.PostAsJsonAsync("api/Payments/pay-invoice", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    _logger.LogInformation($"Ödeme başarılı. Yeni bakiye: {result?.newBalance}");
                    return (true, (decimal?)result?.newBalance);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Ödeme başarısız. Status: {response.StatusCode}, Error: {error}");
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme işlemi sırasında bir hata oluştu");
                return (false, null);
            }
        }
    }

    public class PaymentResponse
    {
        public bool Success { get; set; }
        public decimal NewBalance { get; set; }
    }
} 
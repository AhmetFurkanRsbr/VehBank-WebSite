using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using VEHABANK.Shared.Dto;

namespace VEHABANK.WebUI.Controllers
{
    public class UserPageController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserPageController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("VEHABANK.WebApi");
        }

        public async Task<IActionResult> Index()
        {
            //var userId = HttpContext.Session.GetInt32("userId"); 
            var userId = 1016;
            Console.WriteLine("User ıd session dan : "+userId);
            var response = await _httpClient.GetAsync($"https://localhost:7060/api/Account/GetAccountsByUserId/${userId}");


            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AccountDto>()); // Boş liste gönder
            }

            var json = await response.Content.ReadAsStringAsync();
            var accounts = JsonSerializer.Deserialize<List<AccountDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(accounts);
        }
    }
}

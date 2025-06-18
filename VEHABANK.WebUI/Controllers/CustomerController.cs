using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using VEHABANK.Shared.Dto;

namespace VEHABANK.WebUI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> DepositMoney(TransferDto dto)
        {
            var client = _httpClientFactory.CreateClient("VehaBankApi");
            var response = await client.PostAsJsonAsync("api/Customer/DepositMoney", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Para yatırma işlemi başarılı.";
                return RedirectToAction("Index","UserPage");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Para yatırma başarısız: " + error);
            return View(dto);
        }

        public IActionResult DepositMoney()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferDto dto)
        {
            var client = _httpClientFactory.CreateClient("VehaBankApi");
            var response = await client.PostAsJsonAsync("api/Customer/Transfer", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Para transferi işlemi başarılı.";
                return RedirectToAction("Index", "UserPage");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Para transferi başarısız: " + error);
            return View(dto);
        }

        public IActionResult Transfer()
        {
            return View();
        }

    }
}

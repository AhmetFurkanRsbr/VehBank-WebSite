//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using VEHABANK.Shared;

namespace VEHABANK.WebUI.Controllers
{
    //[Authorize(Roles = "BankTeller")]
    public class OpenAccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenAccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult OpenBankAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OpenBankAccount(OpenAccountDto dto)
        {
            var token = Request.Cookies["accessToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Console.WriteLine("mmmmmmmmmmmmmmmmmmm           "+jwtToken);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bilgisi alınamadı.");
                return View(dto);
            }

            dto.UserId = int.Parse(userIdClaim.Value);

            var client = _httpClientFactory.CreateClient("api");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("api/OpenAccount", dto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Hesap başarıyla açıldı.";
                return RedirectToAction("OpenBankAccount");
            }

            ModelState.AddModelError(string.Empty, "Hesap oluşturulamadı.");
            return View(dto);
        }


        //Kredi kartı hesabı açma

        [HttpGet]
        public IActionResult OpenCreditAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OpenCreditAccount(OpenAccountDto dto)
        {
            var token = Request.Cookies["accessToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bilgisi alınamadı.");
                return View(dto);
            }

            dto.UserId = int.Parse(userIdClaim.Value);

            var client = _httpClientFactory.CreateClient("api");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("api/OpenCreditAccount", dto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Hesap başarıyla açıldı.";
                return RedirectToAction("OpenCreditAccount");
            }

            ModelState.AddModelError(string.Empty, "Hesap oluşturulamadı.");
            return View(dto);
        }



    }
}

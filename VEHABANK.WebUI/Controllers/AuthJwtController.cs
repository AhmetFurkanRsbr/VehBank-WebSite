using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using VEHABANK.WebUI.Models;

namespace VEHABANK.WebUI.Controllers
{
    public class AuthJwtController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthJwtController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("VEHABANK.WebApi");
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                //var response = await httpClient.PostAsync("https://localhost:7060/api/AuthenticationJwt/Login", jsonContent);
                var response = await httpClient.PostAsync("https://localhost:7060/api/AuthenticationJwt/Login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);
                    //var token2 = doc.RootElement.GetProperty("token").GetString();
                    var token2 = doc.RootElement.GetProperty("jwtToken").GetString();
                    var userId = doc.RootElement.GetProperty("userId").GetInt32();

                    Console.WriteLine("mmmmmmmmmmmmmmmmmmmmmmmm: "+token2);   
                    HttpContext.Session.SetString("jwtToken", token2);
                    HttpContext.Session.SetInt32("userId", userId);


                    var token = await response.Content.ReadAsStringAsync();
                    token = token.Trim('"');

                    return Json(new { success = true, token = token });
                }

                return Json(new { success = false, message = "Giriş başarısız." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Giriş sırasında bir hata oluştu." });
            }
        }

        public IActionResult Logout()
        {
            return Json(new { success = true });
        }

    }

}

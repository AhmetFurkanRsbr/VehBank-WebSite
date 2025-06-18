using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using VEHABANK.Shared.Dto;

namespace VEHABANK.WebUI.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> ManageEmployees()
        {
            var client = _httpClientFactory.CreateClient("VEHABANK.WebApi");
            /*
            var token = HttpContext.Session.GetString("jwtToken");
            Console.WriteLine("mmmmmmmmmmmmmmmm: "+token);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);*/

            try
            {
                //var response = await client.GetAsync("https://localhost:7060/api/Employees");
                var response = await client.GetAsync("https://localhost:7060/api/Employees");
                response.EnsureSuccessStatusCode(); // burası 401 olursa hata fırlatır

                var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
                return View(employees);
            }
            catch (HttpRequestException ex)
            {
                // Geliştirme aşamasında log için Console da olur
                Console.WriteLine("API çağrısı başarısız: " + ex.Message);
                TempData["ErrorMessage"] = "Sunucuya erişilemedi. Lütfen daha sonra tekrar deneyin.";
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeDto dto)
        {
            var client = _httpClientFactory.CreateClient("VEHABANK.WebApi");
            //var response = await client.PostAsJsonAsync("https://localhost:7060/api/Employees", dto);
            var response = await client.PostAsJsonAsync("https://localhost:7060/api/Employees", dto);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Ekleme başarılı.";

            return View("ManageEmployees");
        }
        public IActionResult AddEmployee()
        {

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var client = _httpClientFactory.CreateClient("VEHABANK.WebApi");
            //var response = await client.DeleteAsync($"https://localhost:7060/api/Employees/Delete/{id}");
            var response = await client.DeleteAsync($"https://localhost:7060/api/Employees/Delete/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Personel başarıyla silindi.";
            else
                TempData["SuccessMessage"] = "Silme işlemi başarısız.";

            return RedirectToAction("ManageEmployees");
        }


        // GET: Düzenleme sayfasını getir
        public async Task<IActionResult> UpdateEmployee(int id)
        {
            var client = _httpClientFactory.CreateClient("VEHABANK.WebApi");
            //var response = await client.GetAsync("https://localhost:7060/api/Employees");
            var response = await client.GetAsync("https://localhost:7060/api/Employees");
            if (!response.IsSuccessStatusCode) return NotFound();

            var list = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
            var employee = list.FirstOrDefault(x => x.Id == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Düzenlenen veriyi API'ye gönder
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(EmployeeUpdateDto dto)
        {
            var client = _httpClientFactory.CreateClient("VEHABANK.WebApi");
            //var response = await client.PostAsJsonAsync("https://localhost:7060/api/Employees/Update", dto);
            var response = await client.PostAsJsonAsync("https://localhost:7060/api/Employees/Update", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Personel güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Güncelleme başarısız.";
            }

            return RedirectToAction("ManageEmployees");
        }

    }

}
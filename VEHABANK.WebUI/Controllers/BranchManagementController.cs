using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using VEHABANK.Shared.Dto;

namespace VEHABANK.WebUI.Controllers
{
    public class BranchManagementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BranchManagementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
      

        public async Task<IActionResult> ManageBranchInformation()
        {

            var client = _httpClientFactory.CreateClient(); // DI ile tanımlanmalı
            //var response = await client.GetAsync("https://localhost:7060/api/Branches/GetAllBranches");
            var response = await client.GetAsync("https://localhost:7060/api/Branches/GetAllBranches");

            if (!response.IsSuccessStatusCode)
            {
                // Hata durumu
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var branchList = JsonConvert.DeserializeObject<List<BranchDto>>(json);

            ViewBag.BranchList = branchList.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            return View();

        }


        [HttpPost]
        public async Task<IActionResult> ManageBranchInformation(IFormCollection form)
        {
            var dto = new UpdateBranchDto
            {
                Id = int.Parse(form["branchId"]),
                City = form["city"],
                Email = form["mail"],
                Phone = form["phone"],
                Address = form["address"]
            };

            var client = _httpClientFactory.CreateClient("VEHABANK.WebApi");
            //var response = await client.PostAsJsonAsync("https://localhost:7060/api/Branches/UpdateBranch", dto);
            var response = await client.PostAsJsonAsync("https://localhost:7060/api/Branches/UpdateBranch", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şube bilgisi başarıyla güncellendi.";
                return RedirectToAction("ManageBranchInformation");
            }

            TempData["ErrorMessage"] = "Şube bilgisi güncellenirken bir hata oluştu.";
            return View();
        }




    }
}


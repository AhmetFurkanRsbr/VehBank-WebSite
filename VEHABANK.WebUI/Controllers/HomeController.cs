using Microsoft.AspNetCore.Mvc;

namespace VEHABANK.WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

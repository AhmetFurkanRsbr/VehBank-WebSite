using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VEHABANK.WebUI.Models;

namespace VEHABANK.WebUI.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Login()
        {
            return ViewComponent("_LoginComponentPartial");
        }

        public IActionResult Register()
        {
            return ViewComponent("_RegisterComponentPartial");
        }
        public IActionResult VerifyEmail()//Email doğrulama işlemleri
        {
            return ViewComponent("_EmailComponentPartial");
        }

    }
}
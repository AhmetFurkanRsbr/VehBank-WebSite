using Microsoft.AspNetCore.Mvc;

namespace VEHABANK.WebUI.Controllers
{
    public class CounterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BlockAccount()
        {
            return View();
        }

        public IActionResult AccountApproval()
        {
            return View();
        }


        [HttpPost]
        public IActionResult BlockAccount(string AccountNumber)
        {
            TempData["SuccessMessage"] = "Hesap başarıyla bloke edildi!";
            return RedirectToAction("BlockAccount");
        }

        [HttpPost]
        public IActionResult AccountApproval(string AccountNumber)
        {
            TempData["SuccessMessage"] = "Hesap başarıyla onaylandı/aktifleştirildi!";
            return RedirectToAction("AccountApproval");
        }

        [HttpPost]
        public IActionResult FeeIntervention(string TransactionNumber, string NewFee)
        {
            TempData["SuccessMessage"] = "İşlem ücreti başarıyla güncellendi!";
            return RedirectToAction("FeeIntervention");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using VEHABANK.WebUI.Services;
using VEHABANK.Shared.DTOs;

namespace VEHABANK.WebUI.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public IActionResult Invoice()
        {
            return View();
        }

        public IActionResult Dormitory()
        {
            return View();
        }

        [HttpPost("pay-invoice")]
        public async Task<IActionResult> PayInvoice([FromBody] PayInvoiceDto request)
        {
            var (success, newBalance) = await _paymentService.PayInvoice(request);
            if (success)
                return Ok(new { success = true, newBalance });
            return BadRequest("Ödeme işlemi başarısız oldu.");
        }
    }
}
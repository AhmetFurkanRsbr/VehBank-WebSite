using Microsoft.AspNetCore.Mvc;

namespace VEHABANK.WebUI.ViewComponents.EmailViewComponents
{
    public class _EmailComponentPartial : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}

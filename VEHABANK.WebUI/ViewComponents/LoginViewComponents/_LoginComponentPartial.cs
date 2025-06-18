using Microsoft.AspNetCore.Mvc;

namespace VEHABANK.WebUI.ViewComponents.LoginViewComponents
{
    public class _LoginComponentPartial: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    
    }
}

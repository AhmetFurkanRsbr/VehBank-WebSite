using Microsoft.AspNetCore.Mvc;

namespace VEHABANK.WebUI.ViewComponents.RegisterViewComponents
{
    public class _RegisterComponentPartial: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    
    }
}

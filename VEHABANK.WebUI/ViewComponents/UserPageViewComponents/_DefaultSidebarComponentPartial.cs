using Microsoft.AspNetCore.Mvc;

namespace VEHABANK.WebUI.ViewComponents.UserPageViewComponents
{
    public class _DefaultSidebarComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

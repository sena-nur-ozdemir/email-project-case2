using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.ViewComponents
{
    public class _NavbarDefaultComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

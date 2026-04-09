using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.ViewComponents
{
    public class _ScriptsDefaultComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

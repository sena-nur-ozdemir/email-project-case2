using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.ViewComponents
{
    public class _ThemeSettingDefaultComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

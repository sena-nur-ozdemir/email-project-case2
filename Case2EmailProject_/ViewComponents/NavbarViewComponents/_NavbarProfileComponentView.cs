using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.ViewComponents.NavbarViewComponents
{
    public class _NavbarProfileComponentView :ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public _NavbarProfileComponentView(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(user);
        }
    }
}

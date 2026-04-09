using Case2EmailProject_.Context;
using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.ViewComponents.NavbarViewComponents
{
    public class _NavbarUnreadMailComponentPartial:ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public _NavbarUnreadMailComponentPartial(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Where(x => x.ReceiverMail == user.Email &&
            !x.IsRead && !x.IsDeletedByReceiver && !x.IsDraft).OrderByDescending(x => x.SendingDate).ToList();
            return View(messages);

        }

    }
}

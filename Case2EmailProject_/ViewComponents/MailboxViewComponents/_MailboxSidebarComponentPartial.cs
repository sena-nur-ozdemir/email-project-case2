using Case2EmailProject_.Context;
using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Case2EmailProject_.ViewComponents.MailboxViewComponents
{
    public class _MailboxSidebarComponentPartial : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public _MailboxSidebarComponentPartial(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.v1 = _context.Messages.Where(x => x.ReceiverMail == user.Email && !x.IsDeletedByReceiver && !x.IsSpam && !x.IsDraft).Count();
            ViewBag.v2 = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.IsStarred == true && !x.IsSpam && !x.IsDeletedByReceiver).Count();
            ViewBag.v3 = _context.Messages.Where(x => x.SenderMail == user.Email && !x.IsDraft && !x.IsDeletedBySender).Count();
            ViewBag.v4 = _context.Messages.Where(x => x.SenderMail == user.Email && x.IsDraft == true && !x.IsDeletedBySender).Count();
            ViewBag.v5 = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.IsSpam == true && !x.IsDeletedByReceiver).Count();
            ViewBag.v6 = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.IsDeletedByReceiver == true).Count();

            return View();
        }
    }
}
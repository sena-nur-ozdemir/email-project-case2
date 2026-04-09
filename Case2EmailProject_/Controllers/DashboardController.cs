using Case2EmailProject_.Context;
using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Case2EmailProject_.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public DashboardController(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Index", "Register");
            }

            ViewBag.sentMessages = _context.Messages.Where(x => x.SenderMail == user.Email && !x.IsDraft && !x.IsDeletedBySender).Count();
            ViewBag.unreadMessages = _context.Messages.Where(x => x.ReceiverMail == user.Email && !x.IsRead && !x.IsDeletedByReceiver && !x.IsDraft).Count();
            ViewBag.spamMessages = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.IsSpam && !x.IsDeletedByReceiver).Count();
            ViewBag.starredMessages = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.IsStarred && !x.IsDeletedByReceiver).Count();
            ViewBag.draftMessages = _context.Messages.Where(x => x.SenderMail == user.Email && x.IsDraft && !x.IsDeletedBySender).Count();
            ViewBag.trashMessages = _context.Messages.Where(x => (x.ReceiverMail == user.Email && x.IsDeletedByReceiver)
            || (x.SenderMail == user.Email && x.IsDeletedBySender)).Count();

            var monthlySent = _context.Messages.Where(x => x.SenderMail == user.Email && !x.IsDraft && !x.IsDeletedBySender).GroupBy(x => x.SendingDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                }).ToList();

            var monthlyInbox = _context.Messages.Where(x => x.ReceiverMail == user.Email && !x.IsDraft && !x.IsDeletedByReceiver)
                .GroupBy(x => x.SendingDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count(),
                }).ToList();

            var categories = _context.Messages.Where(x => x.ReceiverMail == user.Email && !x.IsDeletedByReceiver && !x.IsDraft)
                .GroupBy(x => x.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Count = g.Count()
                })
                .ToList();

            ViewBag.CategoryNames = categories.Select(x => x.CategoryName).ToList();
            ViewBag.CategoryCounts = categories.Select(x => x.Count).ToList();

            ViewBag.monthlySent = monthlySent;
            ViewBag.monthlyInbox = monthlyInbox;

            var messages = _context.Messages.Where(x => x.SenderMail == user.Email && !x.IsDeletedBySender && !x.IsDraft)
                .Include(x => x.Category).Take(3).ToList();

            return View(messages);
        }

    }
}

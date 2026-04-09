using Case2EmailProject_.Context;
using Case2EmailProject_.Dtos;
using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.ViewComponents.MailboxViewComponents
{
    public class _MailboxSidebarCategoryComponentPartial:ViewComponent
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public _MailboxSidebarCategoryComponentPartial(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = _context.Categories.Select(c => new CategoryDto()
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                MessageCount = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.CategoryId == c.CategoryId && !x.IsDeletedByReceiver && !x.IsDraft).Count()
            }).ToList();

            return View(values);
        }
       
    }
}

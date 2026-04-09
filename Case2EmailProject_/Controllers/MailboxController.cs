using Case2EmailProject_.Context;
using Case2EmailProject_.Dtos;
using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Case2EmailProject_.Controllers
{
    public class MailboxController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public MailboxController(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Where(x => x.ReceiverMail == user.Email && !x.IsSpam && !x.IsDeletedByReceiver && !x.IsDraft)
                .Include(x => x.Category)
                .OrderByDescending(x => x.SendingDate).ToList();
            return View(messages);
        }
        public async Task<IActionResult> Sendbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Where(x => x.SenderMail == user.Email && !x.IsDraft && !x.IsDeletedBySender)
                .Include(x => x.Category)
                .OrderByDescending(x => x.SendingDate).ToList();
            return View(messages);
        }
        public async Task<IActionResult> DraftBox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = _context.Messages.Where(x => x.SenderMail == user.Email && x.IsDraft == true && !x.IsDeletedBySender)
                .Include(x => x.Category)
                .OrderByDescending(x => x.SendingDate).ToList();
            return View(values);
        }
        public async Task<IActionResult> SpamList()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.IsSpam == true && !x.IsDeletedByReceiver)
                .Include(x => x.Category)
                .OrderByDescending(x => x.SendingDate).ToList();
            return View(messages);
        }
        public async Task<IActionResult> StarredList()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.IsStarred && !x.IsSpam && !x.IsDeletedByReceiver)
                .Include(x => x.Category).OrderByDescending(x => x.SendingDate).ToList();
            return View(messages);
        }
        public async Task<IActionResult> TrashBox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Where(x => (x.ReceiverMail == user.Email && x.IsDeletedByReceiver)
            || (x.SenderMail == user.Email && x.IsDeletedBySender))
                .OrderByDescending(x => x.SendingDate).Include(x => x.Category).ToList();
            return View(messages);
        }
        public async Task<IActionResult> GetMessagesByCategory(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Where(x => x.ReceiverMail == user.Email && x.CategoryId == id && !x.IsDeletedByReceiver && !x.IsDraft)
                .Include(x => x.Category)
                .OrderByDescending(x => x.SendingDate).ToList();

            ViewBag.activeCategoryId = id;
            return View(messages);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStar(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var message = _context.Messages.Where(x => x.MessageId == id &&
           x.ReceiverMail == user.Email).FirstOrDefault();
            if (message == null)
            {
                return NotFound();
            }
            message.IsStarred = !message.IsStarred;
            _context.Messages.Update(message);
            _context.SaveChanges();
            return Json(new { isStarred = message.IsStarred });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSpam(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var message = _context.Messages.Where(x => x.MessageId == id && x.ReceiverMail == user.Email).FirstOrDefault();
            if (message == null)
            {
                return NotFound();
            }
            message.IsSpam = !message.IsSpam;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return Json(new { isSpam = message.IsSpam });

        }
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var message = _context.Messages.Where(x => x.MessageId == id &&
            (x.ReceiverMail == user.Email || x.SenderMail == user.Email)).FirstOrDefault();
            if (message == null)
            {
                return NotFound();
            }
            if (message.SenderMail == user.Email)
            {
                message.IsDeletedBySender = true;
            }
            if (message.ReceiverMail == user.Email)
            {
                message.IsDeletedByReceiver = true;
            }
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<IActionResult> GetMessageDetails(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var message = _context.Messages.Where(x => x.MessageId == id &&
            (x.ReceiverMail == user.Email || x.SenderMail == user.Email)).Include(x => x.Category).FirstOrDefault();
            if (message.ReceiverMail == user.Email && !message.IsRead)
            {
                message.IsRead = true;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
            }
            bool isInTrash = (message.ReceiverMail == user.Email && message.IsDeletedByReceiver) ||
                                (message.SenderMail == user.Email && message.IsDeletedBySender);

            ViewBag.IsInTrash = isInTrash;
            return View(message);
        }
        [HttpGet]
        public IActionResult SendMessage()
        {
            List<SelectListItem> categoryValues = _context.Categories
                                .Select(x => new SelectListItem
                                {
                                    Text = x.CategoryName,
                                    Value = x.CategoryId.ToString()
                                }).ToList();
            ViewBag.categories = categoryValues;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Message message = new Message
            {
                SenderMail = user.Email,
                ReceiverMail = createMessageDto.ReceiverMail,
                Subject = createMessageDto.Subject,
                MessageDetail = createMessageDto.MessageDetail,
                SendingDate = DateTime.Now,
                CategoryId = createMessageDto.CategoryId,
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("SendBox");
        }

        [HttpPost]
        public async Task<IActionResult> SaveDraft(CreateMessageDto createMessageDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var message = new Message
            {
                SenderMail = user.Email,
                ReceiverMail = createMessageDto.ReceiverMail,
                Subject = createMessageDto.Subject,
                MessageDetail = createMessageDto.MessageDetail,
                SendingDate = DateTime.Now,
                CategoryId = createMessageDto.CategoryId,
                IsDraft = true,
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("DraftBox");
        }

    }
}


using Case2EmailProject_.Dtos;
using Case2EmailProject_.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace Case2EmailProject_.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RegisterController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRegisterDto userRegisterDto)
        {
            AppUser appUser = new AppUser()
            {
                Name = userRegisterDto.Name,
                Surname = userRegisterDto.Surname,
                UserName = userRegisterDto.Username,
                Email = userRegisterDto.Email
            };
            var result = await _userManager.CreateAsync(appUser, userRegisterDto.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            Random random = new Random();
            var code = random.Next(100000, 999999).ToString();

            appUser.EmailConfirmCode = code;
            var updatedResult = await _userManager.UpdateAsync(appUser);
            if (!updatedResult.Succeeded)
            {
                ModelState.AddModelError("", "Doğrulama kodu oluşturulurken bir hata oluştu. Lütfen tekrar deneyiniz.");
                return View();
            }

            try
            {
                MimeMessage mimeMessage = new MimeMessage();

                MailboxAddress mailboxAddressFrom = new MailboxAddress("Identity Admin", "senaapp07@gmail.com");
                mimeMessage.From.Add(mailboxAddressFrom);

                MailboxAddress mailboxAddressTo = new MailboxAddress("User", userRegisterDto.Email);
                mimeMessage.To.Add(mailboxAddressTo);

                mimeMessage.Subject = "Email Doğrulama Kodu";
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = $"Email doğrulama kodunuz: {code}";
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.CheckCertificateRevocation = false; // (Geliştirme ortamı için geçici çözüm)
                smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtpClient.Authenticate("senanur0402@gmail.com", "api key buraya gelecek");
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);

            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Doğrulama maili gönderilemedi");
                return View();
            }
            return RedirectToAction("ConfirmEmail", new { email = userRegisterDto.Email });
        }

        [HttpGet]
        public IActionResult ConfirmEmail(string email)
        {
            return View(new ConfirmEmailDto { Email = email });
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View();
            }
            if (user.EmailConfirmCode != confirmEmailDto.ConfirmCode)
            {
                ModelState.AddModelError("", "Doğrulama kodu yanlış");
                return View();
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Inbox", "Mailbox");
        }

    }
}


using Case2EmailProject_.Dtos;
using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userLoginDto.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View();
            }
            if (!user.EmailConfirmed)
            {
                return RedirectToAction("ConfirmEmail", "Register", new { email = user.Email });
            }

            var result = await _signInManager.PasswordSignInAsync(userLoginDto.Username, userLoginDto.Password, true, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Inbox", "Mailbox");
            }
            ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}

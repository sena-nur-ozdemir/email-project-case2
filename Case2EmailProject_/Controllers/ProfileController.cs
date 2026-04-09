using Case2EmailProject_.Dtos;
using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Case2EmailProject_.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditDto userEditDto = new UserEditDto();
            userEditDto.Name = user.Name;
            userEditDto.Surname = user.Surname;
            userEditDto.Email = user.Email;
            userEditDto.ImageUrl = user.ImageUrl;
            return View(userEditDto);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserEditDto userEditDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.Name = userEditDto.Name;
            user.Surname = userEditDto.Surname;
            user.Email = userEditDto.Email;
            if (!string.IsNullOrEmpty(userEditDto.Password))
            {
                if (userEditDto.Password == userEditDto.ConfirmPassword)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userEditDto.Password);
                }
                else
                {
                    ModelState.AddModelError("", "Şifreler uyuşmuyor");
                    return View(userEditDto);
                }
            }
            if (userEditDto.Image != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(userEditDto.Image.FileName);
                var imageName = Guid.NewGuid() + extension;
                var saveLocation = resource + "/wwwroot/images/" + imageName;
                var stream = new FileStream(saveLocation, FileMode.Create);
                await userEditDto.Image.CopyToAsync(stream);
                user.ImageUrl = imageName;
            }
            else
            {
                user.ImageUrl = userEditDto.ImageUrl;
            }
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Inbox", "Mailbox");

            }
            return View(userEditDto);
        }
    }
}

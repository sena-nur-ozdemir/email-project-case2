using Microsoft.AspNetCore.Identity;

namespace Case2EmailProject_.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
        public string? About { get; set; }
        public string? EmailConfirmCode { get; set; }
    }
}


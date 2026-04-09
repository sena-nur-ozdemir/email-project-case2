using Case2EmailProject_.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Case2EmailProject_.Context
{
    public class EmailContext:IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-N905OEG\\SQLEXPRESS;initial catalog=Case2DbEmailProject;integrated security=true;" +
                "trust server certificate=true");
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}

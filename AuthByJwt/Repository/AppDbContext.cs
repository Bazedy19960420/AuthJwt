using AuthByJwt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthByJwt.Repository
{
    public class AppDbContext : IdentityDbContext<User>

    {
        public AppDbContext(DbContextOptions options) : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                    new IdentityRole
                    {
                        Name = "Manager",
                        NormalizedName = "MANAGER"
                    },
                     new IdentityRole
                     {
                         Name = "Administrator",
                         NormalizedName = "ADMINISTRATOR"
                     }

                );

        }

    }
}

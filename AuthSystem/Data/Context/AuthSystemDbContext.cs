using AuthSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Data.Context
{
    public class AuthSystemDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthSystemDbContext(DbContextOptions<AuthSystemDbContext> options) : base(options) { }

    }
}

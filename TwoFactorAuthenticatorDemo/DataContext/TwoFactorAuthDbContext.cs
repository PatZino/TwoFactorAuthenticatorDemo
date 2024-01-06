using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TwoFactorAuthenticatorDemo.Models;

namespace TwoFactorAuthenticatorDemo.DataContext
{
    public class TwoFactorAuthDbContext: IdentityDbContext<User>
    {
        public TwoFactorAuthDbContext(DbContextOptions<TwoFactorAuthDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}

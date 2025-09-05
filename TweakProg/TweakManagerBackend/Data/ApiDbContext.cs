using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TweakManagerBackend.Models;

namespace TweakManagerBackend.Data
{
    public class ApiDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<LicenseKey> LicenseKeys { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
    }
}
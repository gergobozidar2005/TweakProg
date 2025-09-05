using Microsoft.AspNetCore.Identity;

namespace TweakManagerBackend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? HardwareId { get; set; }
    }
}
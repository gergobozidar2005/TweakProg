using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TweakManagerBackend.Data;
using TweakManagerBackend.Models;

namespace TweakManagerBackend.Controllers
{
    [Authorize] // Csak bejelentkezett felhasználók érhetik el!
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public RegistrationController(ApiDbContext context)
        {
            _context = context;
        }

        // Adatstruktúra a kliens kéréséhez
        public class ActivateLicenseRequest
        {
            public required string LicenseKey { get; set; }
            public required string HardwareId { get; set; }
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateLicense([FromBody] ActivateLicenseRequest request)
        {
            // Lekérjük a bejelentkezett felhasználó ID-ját
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Megkeressük a kulcsot a Hardver ID alapján
            var license = await _context.LicenseKeys.FirstOrDefaultAsync(k =>
                k.KeyValue == request.LicenseKey &&
                k.AssignedToHardwareId.ToUpper() == request.HardwareId.ToUpper());

            if (license == null)
            {
                return BadRequest("A licenckulcs vagy a hardver ID érvénytelen.");
            }

            // Itt jönnek az ellenőrzések (aktív, lejárt, stb.)
            if (!license.IsActive) return BadRequest("A licenc le van tiltva.");
            if (license.ExpiryDate.HasValue && license.ExpiryDate.Value < DateTime.UtcNow) return BadRequest("A licenc lejárt.");

            // Ellenőrizzük, hogy a kulcsot nem használja-e már más
            if (license.IsUsed && license.ApplicationUserId != userId)
            {
                return BadRequest("Ezt a kulcsot már egy másik fiókhoz aktiválták.");
            }

            // Ha minden rendben, hozzárendeljük a kulcsot a felhasználóhoz
            license.IsUsed = true;
            license.ApplicationUserId = userId;
            await _context.SaveChangesAsync();

            return Ok("Licenc sikeresen aktiválva!");
        }

        // Ellenőrzi, hogy a bejelentkezett felhasználónak van-e már aktív licence
        [HttpGet("check")]
        public async Task<IActionResult> CheckLicense()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasLicense = await _context.LicenseKeys.AnyAsync(k => k.ApplicationUserId == userId && k.IsActive);

            return Ok(new { HasLicense = hasLicense });
        }
    }
}
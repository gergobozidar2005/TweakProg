using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TweakManagerBackend.Data;
using TweakManagerBackend.Models;

namespace TweakManagerBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminApiController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminApiController(ApiDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.HardwareId,
                    u.EmailConfirmed,
                    LicenseCount = _context.LicenseKeys.Count(l => l.ApplicationUserId == u.Id)
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("licenses")]
        public async Task<IActionResult> GetLicenses()
        {
            var licenses = await _context.LicenseKeys
                .Include(l => l.ApplicationUser)
                .Select(l => new
                {
                    l.Id,
                    l.KeyValue,
                    l.AssignedToUsername,
                    l.AssignedToHardwareId,
                    l.IsUsed,
                    l.IsActive,
                    l.CreationDate,
                    l.ExpiryDate,
                    UserEmail = l.ApplicationUser != null ? l.ApplicationUser.Email : null
                })
                .ToListAsync();

            return Ok(licenses);
        }

        [HttpPost("licenses")]
        public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseRequest request)
        {
            var license = new LicenseKey
            {
                KeyValue = GenerateLicenseKey(),
                AssignedToUsername = request.Username,
                AssignedToHardwareId = request.HardwareId,
                IsUsed = false,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = request.ExpiryDate
            };

            _context.LicenseKeys.Add(license);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Message = "License created successfully", 
                LicenseKey = license.KeyValue,
                Id = license.Id 
            });
        }

        [HttpPut("licenses/{id}/activate")]
        public async Task<IActionResult> ActivateLicense(int id)
        {
            var license = await _context.LicenseKeys.FindAsync(id);
            if (license == null)
                return NotFound("License not found");

            license.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "License activated successfully" });
        }

        [HttpPut("licenses/{id}/deactivate")]
        public async Task<IActionResult> DeactivateLicense(int id)
        {
            var license = await _context.LicenseKeys.FindAsync(id);
            if (license == null)
                return NotFound("License not found");

            license.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "License deactivated successfully" });
        }

        [HttpDelete("licenses/{id}")]
        public async Task<IActionResult> DeleteLicense(int id)
        {
            var license = await _context.LicenseKeys.FindAsync(id);
            if (license == null)
                return NotFound("License not found");

            _context.LicenseKeys.Remove(license);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "License deleted successfully" });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalLicenses = await _context.LicenseKeys.CountAsync();
            var activeLicenses = await _context.LicenseKeys.CountAsync(l => l.IsActive);
            var usedLicenses = await _context.LicenseKeys.CountAsync(l => l.IsUsed);

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalLicenses = totalLicenses,
                ActiveLicenses = activeLicenses,
                UsedLicenses = usedLicenses,
                AvailableLicenses = totalLicenses - usedLicenses
            });
        }

        private string GenerateLicenseKey()
        {
            var random = new Random();
            var key = new char[16];
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = chars[random.Next(chars.Length)];
            }
            
            return new string(key);
        }
    }

    public class CreateLicenseRequest
    {
        public required string Username { get; set; }
        public required string HardwareId { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
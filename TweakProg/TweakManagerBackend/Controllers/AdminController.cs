using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TweakManagerBackend.Data;
using TweakManagerBackend.Models;
using TweakManagerBackend.Services;

namespace TweakManagerBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApiDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AdminController(
            ApiDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Dashboard";
            return View("Dashboard");
        }

        public IActionResult Keys()
        {
            ViewData["ActivePage"] = "Keys";
            var allKeys = _context.LicenseKeys.OrderByDescending(k => k.CreationDate).ToList();
            return View("Index", allKeys);
        }

        [HttpGet]
        public IActionResult GenerateKey()
        {
            ViewData["ActivePage"] = "Keys";
            return View();
        }

        [HttpPost]
        public IActionResult GenerateKey(string assignedToUsername, string assignedToHardwareId)
        {
            ViewData["ActivePage"] = "Keys";
            if (string.IsNullOrEmpty(assignedToUsername) || string.IsNullOrEmpty(assignedToHardwareId))
            {
                TempData["ErrorMessage"] = "Minden mező kitöltése kötelező!";
                return View();
            }
            var newKey = new LicenseKey
            {
                KeyValue = $"PabloDEV-{Guid.NewGuid().ToString().ToUpper()}",
                AssignedToUsername = assignedToUsername,
                AssignedToHardwareId = assignedToHardwareId
            };
            _context.LicenseKeys.Add(newKey);
            _context.SaveChanges();
            TempData["SuccessMessage"] = $"Kulcs sikeresen generálva: {newKey.KeyValue}";
            return RedirectToAction("Keys");
        }

        [HttpPost]
        public IActionResult ToggleKeyState(int keyId)
        {
            var key = _context.LicenseKeys.Find(keyId);
            if (key != null)
            {
                key.IsActive = !key.IsActive;
                _context.SaveChanges();
            }
            return RedirectToAction("Keys");
        }

        public IActionResult UserManagement()
        {
            ViewData["ActivePage"] = "UserManagement";
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> InitiateAdminPromotion(string userId)
        {
            ViewData["ActivePage"] = "UserManagement";
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["ErrorMessage"] = "A kiválasztott felhasználó nem léptethető elő, mert nem rendelkezik e-mail címmel.";
                return RedirectToAction("UserManagement");
            }
            var pin = new Random().Next(100000, 999999).ToString();
            TempData["AdminPromotion_UserId"] = user.Id;
            TempData["AdminPromotion_Pin"] = pin;
            var emailBody = $"Hello {user.UserName},<br><br>An administrator has initiated a request to grant you admin privileges. <br>Please provide them with the following confirmation PIN: <h1>{pin}</h1>";
            await _emailSender.SendEmailAsync(user.Email, "Admin Promotion Confirmation", emailBody);
            TempData["InfoMessage"] = $"A PIN-kód elküldve a(z) {user.Email} címre. Kérjük, adja meg a kódot a megerősítéshez.";
            return RedirectToAction("ConfirmAdminPromotion");
        }

        [HttpGet]
        public IActionResult ConfirmAdminPromotion()
        {
            ViewData["ActivePage"] = "UserManagement";
            if (TempData["AdminPromotion_UserId"] == null)
            {
                return RedirectToAction("UserManagement");
            }
            TempData.Keep("AdminPromotion_UserId");
            TempData.Keep("AdminPromotion_Pin");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAdminPromotion(string pin)
        {
            var userId = TempData["AdminPromotion_UserId"] as string;
            var correctPin = TempData["AdminPromotion_Pin"] as string;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(correctPin))
            {
                TempData["ErrorMessage"] = "A megerősítési folyamat lejárt. Kérjük, kezdje újra.";
                return RedirectToAction("UserManagement");
            }
            if (pin == correctPin)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    TempData["SuccessMessage"] = $"{user.UserName} sikeresen adminná léptetve!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "A megadott PIN-kód helytelen. Az adminná tétel megszakítva.";
            }
            return RedirectToAction("UserManagement");
        }

        public IActionResult Logs()
        {
            ViewData["ActivePage"] = "Logs";
            var allLogs = _context.LogEntries.OrderByDescending(l => l.Timestamp).ToList();
            return View(allLogs);
        }
    }
}
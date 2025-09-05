using Microsoft.AspNetCore.Identity;
using TweakManagerBackend.Models;

namespace TweakManagerBackend.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            // Szerepkörök létrehozása
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Alapértelmezett admin felhasználó létrehozása
            var adminSettings = configuration.GetSection("DefaultAdmin");
            var adminUsername = adminSettings["Username"];
            var adminEmail = adminSettings["Email"];
            var adminPassword = adminSettings["Password"];

            if (!string.IsNullOrEmpty(adminUsername) && !string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var newAdmin = new ApplicationUser
                    {
                        UserName = adminUsername,
                        Email = adminEmail,
                        EmailConfirmed = true // Az admin fiókja legyen azonnal aktív
                    };
                    var result = await userManager.CreateAsync(newAdmin, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                }
            }
        }
    }
}
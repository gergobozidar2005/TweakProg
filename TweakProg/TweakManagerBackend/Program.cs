using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TweakManagerBackend.Data;
using TweakManagerBackend.Models;
using TweakManagerBackend.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;

var builder = WebApplication.CreateBuilder(args);

// Adatbázis kapcsolat beállítása
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 4, 28))));

// Identity szolgáltatások hozzáadása, szerepkörökkel kiegészítve
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // << FONTOS: Ez a sor kell a RoleManager működéséhez
    .AddEntityFrameworkStores<ApiDbContext>();

// SMTP beállítások és e-mail küldő szolgáltatás
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Swagger/OpenAPI beállítása
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Indítási ellenőrzések és adatbázis-inicializálás
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    Console.WriteLine("--- Performing startup routines ---");
    try
    {
        // 1. Adatbázis-inicializáló (Admin felhasználó és szerepkörök létrehozása)
        await DbInitializer.Initialize(services);
        logger.LogInformation("Database seeding completed successfully.");

        // 2. Adatbázis kapcsolat ellenőrzése
        var dbContext = services.GetRequiredService<ApiDbContext>();
        if (await dbContext.Database.CanConnectAsync())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ OK ] Database connection successful.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[FAIL] Database connection failed.");
        }

        // 3. SMTP szerver kapcsolatának ellenőrzése
        var smtpSettings = services.GetRequiredService<IOptions<SmtpSettings>>().Value;
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(smtpSettings.Server, smtpSettings.Port, SecureSocketOptions.StartTls);
            // await client.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password);
            await client.DisconnectAsync(true);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ OK ] SMTP server connection successful.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during startup routines.");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] A startup routine failed: {ex.Message}");
    }
    finally
    {
        Console.ResetColor();
        Console.WriteLine("-----------------------------------");
    }
}

// HTTP request pipeline konfigurálása
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
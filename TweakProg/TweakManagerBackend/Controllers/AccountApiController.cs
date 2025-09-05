using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using TweakManagerBackend.Models;
using TweakManagerBackend.Services;

[ApiController]
[Route("api/[controller]")]
public class AccountApiController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AccountApiController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public class ClientRegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string HardwareId { get; set; }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ClientRegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            HardwareId = request.HardwareId
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            if (callbackUrl != null)
            {
                var emailBody = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.";
                await _emailSender.SendEmailAsync(request.Email, "Confirm your Tweak App account", emailBody);
            }

            return Ok(new { Message = "Registration successful. Please check your email to confirm your account." });
        }

        return BadRequest(result.Errors);
    }

    public class ClientLoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string HardwareId { get; set; }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ClientLoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        if (user.HardwareId != request.HardwareId)
        {
            return Unauthorized("Hardware ID does not match.");
        }

        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return Ok(new { Message = "Login successful." });
        }

        return Unauthorized("Invalid email or password.");
    }

    public class ForgotPasswordRequest
    {
        public required string Email { get; set; }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Ne fedjük fel, hogy a felhasználó nem létezik, vagy nincs megerősítve
            return Ok(new { Message = "Password reset email sent." });
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var emailBody = $"Your password reset token is: {code}";
        await _emailSender.SendEmailAsync(request.Email, "Reset Tweak App Password", emailBody);

        return Ok(new { Message = "Password reset email sent." });
    }

    public class ResetPasswordRequest
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest("Invalid request.");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Password reset successful." });
        }

        return BadRequest(result.Errors);
    }
}
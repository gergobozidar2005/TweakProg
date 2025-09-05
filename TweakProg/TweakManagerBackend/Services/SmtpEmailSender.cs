using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace TweakManagerBackend.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<SmtpEmailSender> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            _logger.LogInformation("Attempting to send an email to {ToEmail}", toEmail);

            try
            {
                var email = new MimeMessage();
                email.Sender = new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail);
                email.From.Add(email.Sender);
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlMessage };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                // Connect to the server. We set useSsl to false to allow for insecure connections like on port 25.
                _logger.LogInformation("Connecting to SMTP server at {Server}:{Port}...", _smtpSettings.Server, _smtpSettings.Port);
                await smtp.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, false);
                _logger.LogInformation("Connection successful.");

                // ***MODIFIED LOGIC***
                // Only attempt to authenticate if a username is actually provided in the settings.
                if (!string.IsNullOrWhiteSpace(_smtpSettings.Username))
                {
                    _logger.LogInformation("Authenticating with username: '{Username}'...", _smtpSettings.Username);
                    await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                    _logger.LogInformation("Authentication successful.");
                }

                _logger.LogInformation("Sending email...");
                await smtp.SendAsync(email);
                _logger.LogInformation("Email sent successfully.");

                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log the detailed error if something goes wrong.
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            }
        }
    }
}
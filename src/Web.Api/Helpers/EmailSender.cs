using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Web.Api.Core.Configuration;

namespace Web.Api.Helpers;
public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly EmailSettings _emailSettings;
    public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> emailSettings) => (_logger, _emailSettings) = (logger, emailSettings.Value);
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        //_logger.LogDebug($"UserName: {_emailSettings.UserName}, Password: {_emailSettings.Password}");
        /*
            (1) myaccount.google.com
            (2) Security > Signing in to Google
            (3) Enable 2-Step Verification (Without this, you wont see "App passwords")
            (4) App passwords
                (i) Select "Mail" app
                (ii) Select device "Other"        
        */
        if (!string.IsNullOrEmpty(_emailSettings.UserName) && !string.IsNullOrEmpty(email))
        {
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com", //or another email sender provider
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
            };
            MailMessage msg = new MailMessage(new MailAddress(_emailSettings.UserName), new MailAddress(email));
            msg.Subject = subject;
            msg.Body = htmlMessage;
            msg.IsBodyHtml = true;
            await client.SendMailAsync(msg);
            _logger.LogInformation($"{nameof(EmailSender)} Email sent to {email} successfully!");
        }
        else
            _logger.LogError($"{nameof(EmailSender)} Invalid emails! from: {_emailSettings.UserName}, to: {email}");

    }
}

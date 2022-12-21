using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Web.Api.Models.Configurations;

namespace Web.Api.Helpers;
public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly EmailSettings _emailSettings;
    public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> emailSettings) => (_logger, _emailSettings) = (logger, emailSettings.Value);
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogDebug($"UserName: {_emailSettings.UserName}, Password: {_emailSettings.Password}");
        SmtpClient client = new SmtpClient
        {
            Port = 587,
            Host = "smtp.gmail.com", //or another email sender provider
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
        };
        await client.SendMailAsync(_emailSettings.UserName, email, subject, htmlMessage);
    }
}

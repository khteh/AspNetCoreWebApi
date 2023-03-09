using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Web.Api.Core.Configuration;
using Web.Api.Core.Interfaces.Services;

namespace Web.Api.Core.Helpers;

public class Email : IEmail
{
    private readonly ILogger<Email> _logger;
    private readonly EmailSettings _config;
    public Email(ILogger<Email> logger, IOptions<EmailSettings> config) => (_logger, _config) = (logger, config.Value);
    public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string textBody, string htmlBody)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_config.UserName));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = subject;
        BodyBuilder bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = htmlBody;
        bodyBuilder.TextBody = textBody;
        message.Body = bodyBuilder.ToMessageBody();
        using (SmtpClient client = new SmtpClient())
            try
            {
                await client.ConnectAsync(_config.Server, _config.Port, _config.TLS);
                await client.AuthenticateAsync(_config.UserName, _config.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation($"{nameof(SendEmailAsync)} Successfully sent email to {recipientEmail}");
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(SendEmailAsync)} Exception! {e}");
                return false;
            }
        return true;
    }
}
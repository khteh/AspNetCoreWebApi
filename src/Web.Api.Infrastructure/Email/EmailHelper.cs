using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Web.Api.Core.Configuration;
using Web.Api.Core.Helpers;
using Web.Api.Core.Interfaces.Services;
namespace Web.Api.Infrastructure.Email;
public class EmailHelper : IEmail
{
    private readonly ILogger<EmailHelper> _logger;
    private readonly EmailSettings _config;
    private readonly bool _isProduction;
    public EmailHelper(ILogger<EmailHelper> logger, IOptions<EmailSettings> config, IWebHostEnvironment env) => (_logger, _config, _isProduction) = (logger, config.Value, env.IsProduction());
    public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string textBody, string htmlBody)
    {
        if (EmailValidation.IsValidEmail(recipientEmail) && (!string.IsNullOrEmpty(textBody) || !string.IsNullOrEmpty(htmlBody)))
            try
            {
                MimeMessage message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_config.UserName));
                message.To.Add(MailboxAddress.Parse(recipientEmail));
                message.Subject = subject;
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = htmlBody;
                bodyBuilder.TextBody = textBody;
                message.Body = bodyBuilder.ToMessageBody();
                // Wrap the MimeMessage in an SmtpSendRequest
                SmtpSendRequest request = new SmtpSendRequest(message)
                {
                    EnvelopeId = message.MessageId,
                    DeliveryStatusNotifications = DeliveryStatusNotification.Failure | DeliveryStatusNotification.Delay,
                    DeliveryStatusNotificationType = DeliveryStatusNotificationType.HeadersOnly
                };
                using (SmtpClient client = new SmtpClient())
                {
                    await client.ConnectAsync(_config.Server, _config.Port, _config.TLS);
                    await client.AuthenticateAsync(_config.UserName, _config.Password);
                    try
                    {
                        // https://mimekit.net/docs/html/Sending-Messages-With-SMTP.htm
                        SmtpSendResult result = (SmtpSendResult)await client.SendAsync(new SmtpLenientSendRequest(_logger, message));
                        foreach (MailboxAddress recipient in result.RejectedRecipients)
                            _logger.LogWarning($"{nameof(SendEmailAsync)} Not delivered to: {recipient}");
                    }
                    catch (SmtpCommandException ex) when (ex.ErrorCode == SmtpErrorCode.MessageNotAccepted)
                    {
                        // If *none* of the recipients were accepted, an SmtpCommandException is still thrown.
                        _logger.LogError($"{nameof(SendEmailAsync)} None of the recipients were accepted.");
                    }
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(SendEmailAsync)} Exception! {e}");
                return false;
            }
        else
            _logger.LogError($"{nameof(SendEmailAsync)} Invalid email address: {recipientEmail} or empty message body!");
        return true;
    }

    public async Task<bool> SendEmailsAsync(List<Core.Domain.Email> emails)
    {
        bool result = true;
        await Parallel.ForEachAsync(emails, async (email, token) =>
        {
            result &= await SendEmailAsync(email);
        });
        return result;
    }
    public async Task<bool> SendEmailAsync(Core.Domain.Email email)
    {
        if (email != null && EmailValidation.IsValidEmail(email.Recipient) && (!string.IsNullOrEmpty(email.HtmlBody) || !string.IsNullOrEmpty(email.TextBody)))
            try
            {
                MimeMessage message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_config.UserName));
                message.To.Add(MailboxAddress.Parse(email.Recipient));
                message.Subject = email.Subject;
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = email.HtmlBody;
                bodyBuilder.TextBody = email.TextBody;
                message.Body = bodyBuilder.ToMessageBody();
                // Wrap the MimeMessage in an SmtpSendRequest
                SmtpSendRequest request = new SmtpSendRequest(message)
                {
                    EnvelopeId = message.MessageId,
                    DeliveryStatusNotifications = DeliveryStatusNotification.Failure | DeliveryStatusNotification.Delay,
                    DeliveryStatusNotificationType = DeliveryStatusNotificationType.HeadersOnly
                };
                if (_isProduction)
                    using (SmtpClient client = new SmtpClient())
                    {
                        await client.ConnectAsync(_config.Server, _config.Port, _config.TLS);
                        await client.AuthenticateAsync(_config.UserName, _config.Password);
                        try
                        {
                            // https://mimekit.net/docs/html/Sending-Messages-With-SMTP.htm
                            SmtpSendResult result = (SmtpSendResult)await client.SendAsync(new SmtpLenientSendRequest(_logger, message));
                            foreach (MailboxAddress recipient in result.RejectedRecipients)
                                _logger.LogWarning($"{nameof(SendEmailAsync)} Not delivered to: {recipient}");
                        }
                        catch (SmtpCommandException ex) when (ex.ErrorCode == SmtpErrorCode.MessageNotAccepted)
                        {
                            // If *none* of the recipients were accepted, an SmtpCommandException is still thrown.
                            _logger.LogError($"{nameof(SendEmailAsync)} None of the recipients were accepted.");
                        }
                        await client.DisconnectAsync(true);
                    }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(SendEmailAsync)} Exception! {e}");
            }
        else
            _logger.LogError($"{nameof(SendEmailAsync)} Invalid email address: {email?.Recipient} or empty message body!");
        return false;
    }
}
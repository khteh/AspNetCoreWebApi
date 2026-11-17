using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
namespace Web.Api.Infrastructure.Email;

public class SmtpLenientSendRequest : SmtpSendRequest
{
    private ILogger _logger;
    public SmtpLenientSendRequest(ILogger logger, MimeMessage message) : base(message) => _logger = logger;
    public override void OnRecipientNotAccepted(MailboxAddress recipient, SmtpResponse response)
    {
        // Don't throw; the recipient will be listed in SendResult.RejectedRecipients instead.
        _logger.LogWarning($"{nameof(OnRecipientNotAccepted)} {recipient} was rejected: {response.StatusCode}, {response.ResponseText}");
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Api.Core.Domain;
using Web.Api.Core.DTO;
namespace Web.Api.Core.Interfaces.Services;
public interface IEmail
{
    Task<bool> SendEmailsAsync(List<Email> emails);
    Task<bool> SendEmailAsync(Email email);
    Task<bool> SendEmailAsync(string recipientEmail, string subject, string textBody, string htmlBody);
}
using System.Threading.Tasks;
using Web.Api.Core.DTO;
namespace Web.Api.Core.Interfaces.Services;
public interface IEmail
{
    Task<bool> SendEmailAsync(string recipientEmail, string subject, string textBody, string htmlBody);
}
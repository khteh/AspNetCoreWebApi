using System.Threading.Tasks;
namespace Web.Api.Hubs.Interfaces;
public interface IChat
{
    Task ReceiveMessageFromUser(string user, string message);
    Task ReceiveMessage(string message);
}
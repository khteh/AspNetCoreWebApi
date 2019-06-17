using Microsoft.AspNetCore.SignalR;
using Web.Api.Hubs.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Web.Api.Hubs
{
    //[Authorize] //Need token for this to work
    public class ChatHub : Hub<IChat>
    {
        //public async Task SendMessage(string user, string message) => await Clients.All.SendAsync("Receive Message", user, message);
        public async Task ReceiveMessageFromUser(string user, string message) => await Clients.All.ReceiveMessageFromUser(user, message);
        public async Task ReceiveMessage(string message) => await Clients.All.ReceiveMessage(message);
    }
}
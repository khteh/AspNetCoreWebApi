using Microsoft.AspNetCore.SignalR;

namespace Web.Api
{
    public class NameUserIdProvider : IUserIdProvider
    {
        //Rather than ClaimTypes.Name, you can use any value from the User(such as the Windows SID identifier, etc.).
        public string GetUserId(HubConnectionContext connection) => connection.User?.Identity?.Name;
    }
}
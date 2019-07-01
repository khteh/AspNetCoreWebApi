using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;

namespace Web.Api.IntegrationTests.SignalR
{
    public class ChatHubTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        public ChatHubTests(CustomWebApplicationFactory<Startup> factory) => _factory = factory;
        [Fact]
        public async Task ReceiveMessageTest()
        {
            string echo = string.Empty;
            string message = "Integration Testing in Microsoft AspNetCore SignalR";
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("http://localhost/chatHub", o => {
                                o.HttpMessageHandlerFactory = _ =>  _factory.TestServer.CreateHandler();
                                //o.AccessTokenProvider = 
                                //o.Transports = HttpTransportType.WebSockets;
                                //o.SkipNegotiation = true;
                            }).Build();
            connection.On<string>("ReceiveMessage", i => echo = i);
            await connection.StartAsync();
            Assert.Equal(HubConnectionState.Connected, connection.State);
            await connection.InvokeAsync("ReceiveMessage", message);
            Assert.False(string.IsNullOrEmpty(echo));
            Assert.Equal(message, echo);
        }
        [Fact]
        public async Task ReceiveMessageFromUserTest()
        {
            string user = string.Empty;
            string echo = string.Empty;
            string sender = "Mickey Mouse";
            string message = "Integration Testing in Microsoft AspNetCore SignalR";
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("http://localhost/chatHub", o => {
                                o.HttpMessageHandlerFactory = _ => _factory.TestServer.CreateHandler();
                                //o.Transports = HttpTransportType.WebSockets;
                                //o.SkipNegotiation = true;
                            }).Build();
            connection.On<string, string>("ReceiveMessageFromUser", (u, i) => {user = u; echo = i;});
            await connection.StartAsync();
            Assert.Equal(HubConnectionState.Connected, connection.State);
            await connection.InvokeAsync("ReceiveMessageFromUser", sender, message);
            Assert.False(string.IsNullOrEmpty(user));
            Assert.False(string.IsNullOrEmpty(echo));
            Assert.Equal(message, echo);
            Assert.Equal(sender, user);
        }
    }
}
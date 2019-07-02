using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using Biz4x.Frontend.Web.Integration.Test.SignalR;
using Web.Api.Hubs;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Web.Api.Models.Response;
using Microsoft.AspNetCore.Http.Connections;

namespace Web.Api.IntegrationTests.SignalR
{
    public class ChatHubTests : HubBase<ChatHub>
    {
        private readonly TestServer _testServer;
        public ChatHubTests() : base("/chatHub") { _testServer = TestServer; }
        private async Task<string> AccessTokenProvider()
        {
            HttpClient client = HttpClient();
            Assert.NotNull(client);
            var httpResponse = await client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            LoginResponse response = Serialization.JsonSerializer.DeSerializeObject<LoginResponse>(await httpResponse.Content.ReadAsStringAsync());
            Assert.NotNull(response);
            Assert.NotNull(response.AccessToken);
            Assert.False(string.IsNullOrEmpty(response.AccessToken.Token));
            Assert.False(string.IsNullOrEmpty(response.RefreshToken));
            Assert.Equal(7200,(int)response.AccessToken.ExpiresIn);
            return response.AccessToken.Token;
        }
        [Fact]
        public async Task ReceiveMessageTest()
        {
            string echo = string.Empty;
            string message = "Integration Testing in Microsoft AspNetCore SignalR";
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("http://localhost/chatHub", o => {
                                o.HttpMessageHandlerFactory = _ =>  _testServer.CreateHandler();
                                o.AccessTokenProvider = async () => await AccessTokenProvider(); 
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
                                o.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                                o.AccessTokenProvider = async () => await AccessTokenProvider(); 
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
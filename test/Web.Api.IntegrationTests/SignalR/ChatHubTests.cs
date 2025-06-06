using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Models.Response;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
namespace Web.Api.IntegrationTests.SignalR;
//[Collection(SignalRTestsCollection.Name)]
public class ChatHubTests
{
    private readonly TestServer _testServer;
    public ChatHubTests(CustomWebApplicationFactory<Program> factory) => _testServer = factory.Server;
    private async Task<string> AccessTokenProvider()
    {
        HttpClient client = _testServer.CreateClient();
        Assert.NotNull(client);
        var httpResponse = await client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.LogInRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, Application.Json));
        httpResponse.EnsureSuccessStatusCode();
        LogInResponse response = Serialization.JsonSerializer.DeSerializeObject<LogInResponse>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.NotNull(response.AccessToken);
        Assert.False(string.IsNullOrEmpty(response.AccessToken.Token));
        Assert.False(string.IsNullOrEmpty(response.RefreshToken));
        Assert.Equal(7200, (int)response.AccessToken.ExpiresIn);
        return response.AccessToken.Token;
    }
    [Fact]
    public async Task ReceiveMessageTest()
    {
        AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
        string echo = string.Empty;
        string message = "Integration Testing in Microsoft AspNetCore SignalR";
        HubConnection connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:4433/chatHub", o =>
            {
                o.Transports = HttpTransportType.WebSockets;
                o.AccessTokenProvider = async () => await AccessTokenProvider();
                o.SkipNegotiation = true;
                o.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                o.WebSocketFactory = async (context, cancellationToken) =>
                {
                    var wsClient = _testServer.CreateWebSocketClient();
                    var url = $"{context.Uri}?access_token={await AccessTokenProvider()}";
                    return await wsClient.ConnectAsync(new Uri(url), cancellationToken);
                };
            }).Build();
        connection.On<string>("ReceiveMessage", i =>
        {
            echo = i;
            messageReceivedEvent.Set();
        });
        await connection.StartAsync(TestContext.Current.CancellationToken);
        Assert.Equal(HubConnectionState.Connected, connection.State);
        await connection.InvokeAsync("ReceiveMessage", message, TestContext.Current.CancellationToken);
        messageReceivedEvent.WaitOne();
        Assert.False(string.IsNullOrEmpty(echo));
        Assert.Equal(message, echo);
    }
    [Fact]
    public async Task ReceiveMessageFromUserTest()
    {
        AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
        string user = string.Empty;
        string echo = string.Empty;
        string sender = "Mickey Mouse";
        string message = "Integration Testing in Microsoft AspNetCore SignalR";
        HubConnection connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:4433/chatHub", o =>
            {
                o.Transports = HttpTransportType.WebSockets;
                o.AccessTokenProvider = async () => await AccessTokenProvider();
                o.SkipNegotiation = true;
                o.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                o.WebSocketFactory = async (context, cancellationToken) =>
                {
                    var wsClient = _testServer.CreateWebSocketClient();
                    var url = $"{context.Uri}?access_token={await AccessTokenProvider()}";
                    return await wsClient.ConnectAsync(new Uri(url), cancellationToken);
                };
            }).Build();
        connection.On<string, string>("ReceiveMessageFromUser", (u, i) =>
        {
            user = u;
            echo = i;
            messageReceivedEvent.Set();
        });
        await connection.StartAsync(TestContext.Current.CancellationToken);
        Assert.Equal(HubConnectionState.Connected, connection.State);
        await connection.InvokeAsync("ReceiveMessageFromUser", sender, message, TestContext.Current.CancellationToken);
        messageReceivedEvent.WaitOne();
        Assert.False(string.IsNullOrEmpty(user));
        Assert.False(string.IsNullOrEmpty(echo));
        Assert.Equal(message, echo);
        Assert.Equal(sender, user);
    }
}
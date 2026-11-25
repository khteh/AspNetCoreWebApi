using Elastic.CommonSchema;
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

public class ChatHubTests
{
    private readonly HttpClient _client;
    private readonly TestServer _testServer;
    private readonly ITestOutputHelper _output;
    public ChatHubTests(ITestOutputHelper output, CustomWebApplicationFactory<Program> factory)
    {
        _output = output;
        _client = factory.CreateHttpsClient();
        _testServer = factory.Server;
    }
    private async Task<string> AccessTokenProvider()
    {
        Assert.NotNull(_client);
        var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.LogInRequest("testuser", "P@$$w0rd")), Encoding.UTF8, Application.Json));
        httpResponse.EnsureSuccessStatusCode();
        LogInResponse response = Serialization.JsonSerializer.DeSerializeObject<LogInResponse>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.NotNull(response.AccessToken);
        Assert.False(string.IsNullOrEmpty(response.AccessToken.Token));
        Assert.False(string.IsNullOrEmpty(response.RefreshToken));
        Assert.Equal(600, (int)response.AccessToken.ExpiresIn);
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
                // Negotiation is required for authentication refresh (connection token + token lifetime come from the negotiate response).
                o.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                o.WebSocketFactory = async (context, cancellationToken) =>
                {
                    var wsClient = _testServer.CreateWebSocketClient();
                    string separator = string.IsNullOrEmpty(context.Uri.Query) ? "?" : "&"; // negotiate adds ?id=<connection token>
                    var url = $"{context.Uri}{separator}access_token={Uri.EscapeDataString(await AccessTokenProvider())}";
                    return await wsClient.ConnectAsync(new Uri(url), cancellationToken);
                };
            }).WithAuthenticationRefresh(options =>
            {
                // EnableAutoRefresh is true by default.
                options.RefreshBeforeExpiration = TimeSpan.FromMinutes(1);
            }).Build();
        connection.On<string>("ReceiveMessage", i =>
        {
            echo = i;
            messageReceivedEvent.Set();
        });
        connection.AuthenticationRefreshed += context => Task.CompletedTask;
        connection.AuthenticationRefreshFailed += context => Task.CompletedTask;
        await connection.StartAsync(TestContext.Current.CancellationToken);
        Assert.Equal(HubConnectionState.Connected, connection.State);
        // Refresh immediately after acquiring a token with updated claims.
        await connection.RefreshAuthenticationAsync(TestContext.Current.CancellationToken);
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
                // Negotiation is required for authentication refresh (connection token + token lifetime come from the negotiate response).
                o.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                o.WebSocketFactory = async (context, cancellationToken) =>
                {
                    var wsClient = _testServer.CreateWebSocketClient();
                    string separator = string.IsNullOrEmpty(context.Uri.Query) ? "?" : "&"; // negotiate adds ?id=<connection token>
                    var url = $"{context.Uri}{separator}access_token={Uri.EscapeDataString(await AccessTokenProvider())}";
                    return await wsClient.ConnectAsync(new Uri(url), cancellationToken);
                };
            }).WithAuthenticationRefresh(options =>
            {
                // EnableAutoRefresh is true by default.
                options.RefreshBeforeExpiration = TimeSpan.FromMinutes(1);
            }).Build();
        connection.On<string, string>("ReceiveMessageFromUser", (u, i) =>
        {
            user = u;
            echo = i;
            messageReceivedEvent.Set();
        });
        connection.AuthenticationRefreshed += context => Task.CompletedTask;
        connection.AuthenticationRefreshFailed += context => Task.CompletedTask;
        await connection.StartAsync(TestContext.Current.CancellationToken);
        Assert.Equal(HubConnectionState.Connected, connection.State);
        // Refresh immediately after acquiring a token with updated claims.
        await connection.RefreshAuthenticationAsync(TestContext.Current.CancellationToken);
        await connection.InvokeAsync("ReceiveMessageFromUser", sender, message, TestContext.Current.CancellationToken);
        messageReceivedEvent.WaitOne();
        Assert.False(string.IsNullOrEmpty(user));
        Assert.False(string.IsNullOrEmpty(echo));
        Assert.Equal(message, echo);
        Assert.Equal(sender, user);
    }
}
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using System;
using Web.Api.Hubs;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Web.Api.IntegrationTests.SignalR
{
    public class ChatHubTests
    {
        [Fact]
        public async Task ReceiveMessageTest()
        {
            string echo = string.Empty;
            string message = "Integration Testing in Microsoft AspNetCore SignalR";
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                            .ConfigureServices(services => services.AddSignalR())
                            //.Configure(app => app.UseSignalR(routes => routes.MapHub<ChatHub>("/chatHub", options => options.Transports = HttpTransportType.WebSockets)));
                            .Configure(app => app.UseSignalR(routes => routes.MapHub<ChatHub>("/chatHub")));
            TestServer server = new TestServer(webHostBuilder);
            #if false
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("ws://localhost:5000/chatHub", HttpTransportType.WebSockets, o => {
                                o.HttpMessageHandlerFactory = _ => server.CreateHandler();
                                o.Transports = HttpTransportType.WebSockets;
                                o.SkipNegotiation = true;
                            }).Build();
            #else
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("http://localhost/chatHub", o => {
                                o.HttpMessageHandlerFactory = _ => server.CreateHandler();
                                //o.Transports = HttpTransportType.WebSockets;
                                //o.SkipNegotiation = true;
                            }).Build();
            #endif
            connection.On<string>("ReceiveMessage", i => echo = i);
            await connection.StartAsync();
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
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                            .ConfigureServices(services => services.AddSignalR())
                            //.Configure(app => app.UseSignalR(routes => routes.MapHub<ChatHub>("/chatHub", options => options.Transports = HttpTransportType.WebSockets)));
                            .Configure(app => app.UseSignalR(routes => routes.MapHub<ChatHub>("/chatHub")));
            TestServer server = new TestServer(webHostBuilder);
            #if false
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("ws://localhost:5000/chatHub", HttpTransportType.WebSockets, o => {
                                o.HttpMessageHandlerFactory = _ => server.CreateHandler();
                                o.Transports = HttpTransportType.WebSockets;
                                o.SkipNegotiation = true;
                            }).Build();
            #else
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("http://localhost/chatHub", o => {
                                o.HttpMessageHandlerFactory = _ => server.CreateHandler();
                                //o.Transports = HttpTransportType.WebSockets;
                                //o.SkipNegotiation = true;
                            }).Build();
            #endif
            connection.On<string, string>("ReceiveMessageFromUser", (u, i) => {user = u; echo = i;});
            await connection.StartAsync();
            await connection.InvokeAsync("ReceiveMessageFromUser", sender, message);
            Assert.False(string.IsNullOrEmpty(user));
            Assert.False(string.IsNullOrEmpty(echo));
            Assert.Equal(message, echo);
            Assert.Equal(sender, user);
        }
    }
}
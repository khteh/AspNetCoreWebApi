using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Threading.Tasks;
using Web.Api.Hubs;
using Web.Api.Hubs.Interfaces;
using Xunit;

namespace Web.Api.UnitTests.SignalR
{
    public class ChatHubTests
    {
        [Fact]
        public async Task ReceiveMessageTest()
        {
            // arrange
            Mock<IHubCallerClients<IChat>> mockClients = new Mock<IHubCallerClients<IChat>>();
            Mock<IChat> mockClientProxy = new Mock<IChat>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            ChatHub hub = new ChatHub()
            {
                Clients = mockClients.Object
            };
 
            // act
            await hub.ReceiveMessage("Hello World!!!");
 
            // assert
            mockClients.Verify(clients => clients.All, Times.Once);
 
            mockClientProxy.Verify(
                clientProxy => clientProxy.ReceiveMessage(It.Is<string>(o => !string.IsNullOrEmpty(o))),
                Times.Once);
        }
        [Fact]
        public async Task ReceiveMessageFromUserTest()
        {
            // arrange
            Mock<IHubCallerClients<IChat>> mockClients = new Mock<IHubCallerClients<IChat>>();
            Mock<IChat> mockClientProxy = new Mock<IChat>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            ChatHub hub = new ChatHub()
            {
                Clients = mockClients.Object
            };
 
            // act
            await hub.ReceiveMessageFromUser("Mickey Mouse", "Hello World!!!");
 
            // assert
            mockClients.Verify(clients => clients.All, Times.Once);
 
            mockClientProxy.Verify(
                clientProxy => clientProxy.ReceiveMessageFromUser(It.Is<string>(o => !string.IsNullOrEmpty(o)), It.Is<string>(o => !string.IsNullOrEmpty(o))),
                Times.Once);
        }
    }
}
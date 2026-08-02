using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Numerics;
using System.ComponentModel;
using Moq;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Presenters;
using Web.Api.MCPServer;
using Xunit;
using Web.Api.Core.Domain;
namespace Web.Api.UnitTests.MCPServerTools;

public class MCPServerToolsTests
{
    [Fact]
    public static async Task EchoTest()
    {
        // arrange
        var loggerMock = new Mock<ILogger<MCPServer.MCPServerTools>>();
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "echo" }, new CallToolRequestParams() { Name = "echo" });
        // act
        string message = "Hello, World!";
        MCPServer.MCPServerTools mCPServerTools = new MCPServer.MCPServerTools(loggerMock.Object);
        string result = await mCPServerTools.Echo(mcpServer.Object, requestContext, message);
        // assert
        Assert.Equal(message, result);
    }
    [Fact]
    public static async Task GreetTest()
    {
        // arrange
        var loggerMock = new Mock<ILogger<MCPServer.MCPServerTools>>();
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "greet" }, new CallToolRequestParams() { Name = "greet" });
        // act
        string name = "Mickey Mouse";
        MCPServer.MCPServerTools mCPServerTools = new MCPServer.MCPServerTools(loggerMock.Object);
        string result = await mCPServerTools.Greet(mcpServer.Object, requestContext, name);
        // assert
        Assert.Equal($"Hello, {name}!", result);
    }
    [Fact]
    public static async Task AddNumbersTest()
    {
        // arrange
        var loggerMock = new Mock<ILogger<MCPServer.MCPServerTools>>();
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "add_numbers" }, new CallToolRequestParams() { Name = "add_numbers" });
        // act
        decimal a = 5;
        decimal b = 10;
        MCPServer.MCPServerTools mCPServerTools = new MCPServer.MCPServerTools(loggerMock.Object);
        decimal result = await mCPServerTools.AddNumbers(mcpServer.Object, requestContext, a, b);
        // assert
        Assert.Equal(a + b, result);
    }
    [Fact]
    public static async Task FibonacciNegativeNumberShouldThrowTest()
    {
        // arrange
        var loggerMock = new Mock<ILogger<MCPServer.MCPServerTools>>();
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "fibonacci" }, new CallToolRequestParams() { Name = "fibonacci" });
        // act
        MCPServer.MCPServerTools mCPServerTools = new MCPServer.MCPServerTools(loggerMock.Object);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await mCPServerTools.Fibonacci(mcpServer.Object, requestContext, -1));
    }
    [Fact]
    public static async Task FibonacciSuccessTest()
    {
        // arrange
        var loggerMock = new Mock<ILogger<MCPServer.MCPServerTools>>();
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "fibonacci" }, new CallToolRequestParams() { Name = "fibonacci" });
        // act
        MCPServer.MCPServerTools mCPServerTools = new MCPServer.MCPServerTools(loggerMock.Object);
        BigInteger result = await mCPServerTools.Fibonacci(mcpServer.Object, requestContext, 25);
        // assert
        Assert.Equal(75025, result);
    }
    [Fact]
    public static async Task GetSystemInformationTest()
    {
        // act
        var loggerMock = new Mock<ILogger<MCPServer.MCPServerTools>>();
        MCPServer.MCPServerTools mCPServerTools = new MCPServer.MCPServerTools(loggerMock.Object);
        SystemInformation result = await mCPServerTools.GetSystemInformation();
        // assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.OperatingSystem));
        Assert.False(string.IsNullOrEmpty(result.Architecture));
        Assert.False(string.IsNullOrEmpty(result.FrameworkDescription));
        Assert.True(result.ProcessorCount > 0);
        Assert.True(result.TotalMemoryMB > 0);
        Assert.True(result.TotalMemoryAllocatedMB > 0);
        Assert.True(result.UptimeSeconds >= 0);
    }
    [Fact]
    public static async Task GetDiskInfoTest()
    {
        // arrange
        var loggerMock = new Mock<ILogger<MCPServer.MCPServerTools>>();
        MCPServer.MCPServerTools mCPServerTools = new MCPServer.MCPServerTools(loggerMock.Object);
        // act
        List<DiskDriveInfo> result = await mCPServerTools.GetDiskInfo();
        // assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.DoesNotContain(result, d => string.IsNullOrEmpty(d.DriveName));
        Assert.DoesNotContain(result, d => string.IsNullOrEmpty(d.DriveType));
        Assert.True(result.All(d => d.TotalSizeGB >= 0));
        Assert.True(result.All(d => d.AvailableFreeSpaceGB >= 0));
    }
}

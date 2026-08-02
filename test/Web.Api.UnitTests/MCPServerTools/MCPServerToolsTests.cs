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
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "echo" }, new CallToolRequestParams() { Name = "echo" });
        // act
        string message = "Hello, World!";
        string result = await MCPServer.MCPServerTools.Echo(mcpServer.Object, requestContext, message);
        // assert
        Assert.Equal(message, result);
    }
    [Fact]
    public static async Task GreetTest()
    {
        // arrange
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "greet" }, new CallToolRequestParams() { Name = "greet" });
        // act
        string name = "Mickey Mouse";
        string result = await MCPServer.MCPServerTools.Greet(mcpServer.Object, requestContext, name);
        // assert
        Assert.Equal($"Hello, {name}!", result);
    }
    [Fact]
    public static async Task AddNumbersTest()
    {
        // arrange
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "add_numbers" }, new CallToolRequestParams() { Name = "add_numbers" });
        // act
        decimal a = 5;
        decimal b = 10;
        decimal result = await MCPServer.MCPServerTools.AddNumbers(mcpServer.Object, requestContext, a, b);
        // assert
        Assert.Equal(a + b, result);
    }
    [Fact]
    public static async Task FibonacciNegativeNumberShouldThrowTest()
    {
        // arrange
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "fibonacci" }, new CallToolRequestParams() { Name = "fibonacci" });
        // act
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await MCPServer.MCPServerTools.Fibonacci(mcpServer.Object, requestContext, -1));
    }
    [Fact]
    public static async Task FibonacciSuccessTest()
    {
        // arrange
        var mcpServer = new Mock<McpServer>();
        var requestContext = new RequestContext<CallToolRequestParams>(mcpServer.Object, new JsonRpcRequest() { Method = "fibonacci" }, new CallToolRequestParams() { Name = "fibonacci" });
        // act
        BigInteger result = await MCPServer.MCPServerTools.Fibonacci(mcpServer.Object, requestContext, 25);
        // assert
        Assert.Equal(75025, result);
    }
    [Fact]
    public static async Task GetSystemInformationTest()
    {
        // act
        SystemInformation result = await MCPServer.MCPServerTools.GetSystemInformation();
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
        // act
        List<DiskDriveInfo> result = await MCPServer.MCPServerTools.GetDiskInfo();
        // assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.DoesNotContain(result, d => string.IsNullOrEmpty(d.DriveName));
        Assert.DoesNotContain(result, d => string.IsNullOrEmpty(d.DriveType));
        Assert.True(result.All(d => d.TotalSizeGB >= 0));
        Assert.True(result.All(d => d.AvailableFreeSpaceGB >= 0));
    }
}

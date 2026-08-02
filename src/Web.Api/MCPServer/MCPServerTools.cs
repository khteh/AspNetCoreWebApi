using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Numerics;
using System.ComponentModel;
namespace Web.Api.MCPServer;

[McpServerToolType]
public class MCPServerTools
{
    [McpServerTool, Description("Echo back the input message.")]
    public static async Task<string> Echo(McpServer server, RequestContext<CallToolRequestParams> context, string message) => message;
    [McpServerTool, Description("Greets the user with a personalized message.")]
    public static async Task<string> Greet(McpServer server, RequestContext<CallToolRequestParams> context, string name) => $"Hello, {name}!";
    [McpServerTool, Description("Add the 2 input numbers")]
    public static async Task<decimal> AddNumbers(McpServer server, RequestContext<CallToolRequestParams> context, decimal a, decimal b) => a + b;
    [McpServerTool, Description("Return the nth Fibonacci number.")]
    public static async Task<BigInteger> Fibonacci(McpServer server, RequestContext<CallToolRequestParams> context, int n)
    { 
        if (n < 0) 
            throw new ArgumentOutOfRangeException(nameof(n));
        if (n <= 1) 
            return 1;
        BigInteger a = 0, b = 1;
        for (int i = 2; i <= n; i++)
        {
            BigInteger tmp = a + b;
            a = b;
            b = tmp;
        }
        return b;
    }
}

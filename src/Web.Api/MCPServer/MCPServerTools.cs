using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Web.Api.Core.Domain;
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
    [McpServerTool]
    [Description("Retrieves hardware, OS, and environmental system information from the host server.")]
    public static async Task<SystemInformation> GetSystemInformation()
    {
        var process = Process.GetCurrentProcess();
        return new SystemInformation
        {
            OperatingSystem = RuntimeInformation.OSDescription,
            Architecture = RuntimeInformation.OSArchitecture.ToString(),
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            ProcessorCount = Environment.ProcessorCount,
            TotalMemoryAllocatedMB = GC.GetTotalMemory(false) / (1024 * 1024),
            TotalMemoryMB = GC.GetTotalMemory(false) / (1024 * 1024),
            UptimeSeconds = (DateTime.UtcNow - process.StartTime.ToUniversalTime()).TotalSeconds
        };
    }
    [McpServerTool]
    [Description("Returns available and total storage space for all ready drives.")]
    public static async Task<List<DiskDriveInfo>> GetDiskInfo()
    {
        var drives = new List<DiskDriveInfo>();
        foreach (var drive in DriveInfo.GetDrives())
            if (drive.IsReady)
            {
                drives.Add(new DiskDriveInfo
                {
                    DriveName = drive.Name,
                    DriveType = drive.DriveType.ToString(),
                    TotalSizeGB = drive.TotalSize / (1024 * 1024 * 1024),
                    AvailableFreeSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024)
                });
            }
        return drives;
    }
}

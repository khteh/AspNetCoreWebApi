using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Web.Api.Ping;
namespace Web.Api.Services;
public class PingService : SvcPing.SvcPingBase
{
    private readonly ILogger<PingService> _logger;
    public PingService(ILogger<PingService> logger) => _logger = logger;
    public override Task<Pong> Ping(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new Pong
        {
            Message = "Hello!"
        });
    }
}
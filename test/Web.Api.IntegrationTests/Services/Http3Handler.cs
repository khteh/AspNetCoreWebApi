using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Web.Api.IntegrationTests.Services;

public class Http3Handler : DelegatingHandler
{
    public Http3Handler() { }
    public Http3Handler(HttpMessageHandler innerHandler) : base(innerHandler) { }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Version = HttpVersion.Version30;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
        return await base.SendAsync(request, cancellationToken);
    }
}
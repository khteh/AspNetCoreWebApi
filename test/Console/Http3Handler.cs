using System.Net;

namespace BulkUploadConsole;

/// <summary>
/// A delegating handler that changes the request HTTP version to HTTP/3.
/// </summary>
public class Http3Handler : DelegatingHandler
{
    public Http3Handler() { }
    public Http3Handler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Version = HttpVersion.Version30;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
        return base.SendAsync(request, cancellationToken);
    }
}
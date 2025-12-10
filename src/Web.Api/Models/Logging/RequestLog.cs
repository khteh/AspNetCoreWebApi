namespace Web.Api.Models.Logging;

public record RequestLog
{
    public string Method { get; init; }
    public string Scheme { get; init; }
    public string PathBase { get; init; }
    public string Path { get; init; }
    public string Host { get; init; }
    public string Protocol { get; init; }
#if false
    /* The following should have been handled by Serilog.Enrichers.HttpContext configured in appsettings.json
     * https://github.com/denis-peshkov/Serilog.Enrichers.HttpContext
     * https://www.nuget.org/packages/Serilog.Enrichers.HttpContext : outputTemplate: "[{Timestamp:HH:mm:ss}] {Level:u3} ClientIP: {ClientIp} CorrelationId: {CorrelationId} header-name: {headername} {Message:lj}{NewLine}{Exception}"
     */
    public string IP { get; init; }
    public string LocalIP { get; init; }
    public string QueryString { get; init; }
    public long ContentLength { get; init; }
    public string ContentType { get; init; }
    public string UserAgent { get; init; }
    public string X_Forwarded_For { get; init; }
    public string X_Forwarded_Proto { get; init; }
    public string X_Forwarded_Host { get; init; }
    public string X_Original_For { get; init; }
    public string X_Original_Proto { get; init; }
    public string X_Original_Host { get; init; }
    public RequestLog(string method, string scheme, string pathBase, string path, string host, long? length, string ip, string queryString, string contentType, string protocol, IHeaderDictionary headers)
#endif
    public RequestLog(string method, string scheme, string protocol, string pathBase, string path, string host)
    {
        Method = method;
        Scheme = scheme;
        PathBase = pathBase;
        Path = path;
        Host = host;
        Protocol = protocol;
#if false
        IP = ip;
        QueryString = queryString;
        ContentLength = length ?? 0;
        ContentType = contentType;
        if (headers != null)
            foreach (var header in headers)
                if (header.Key.Equals("User-Agent"))
                    UserAgent = header.Value;
                else if (header.Key.Equals("X-Forwarded-For"))
                    X_Forwarded_For = header.Value;
                else if (header.Key.Equals("X-Original-For"))
                {
                    X_Original_For = header.Value;
                    LocalIP = header.Value;
                }
                else if (header.Key.Equals("X-Forwarded-Proto"))
                    X_Forwarded_Proto = header.Value;
                else if (header.Key.Equals("X-Original-Proto"))
                    X_Original_Proto = header.Value;
                else if (header.Key.Equals("X-Forwarded-Host"))
                    X_Forwarded_Host = header.Value;
                else if (header.Key.Equals("X-Original-Host"))
                    X_Original_Host = header.Value;
        //if (string.IsNullOrEmpty(IP) && !string.IsNullOrEmpty(X_Forwarded_For))
        //    IP = X_Forwarded_For;
#endif
    }
}
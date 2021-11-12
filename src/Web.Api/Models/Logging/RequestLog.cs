using Microsoft.AspNetCore.Http;
namespace Web.Api.Models.Logging;
public record RequestLog
{
    public string Method {get; init;}
    public string Scheme {get; init;}
    public string PathBase {get; init;}
    public string Path {get; init;}
    public string LocalIP {get; init;}
    public string IP {get; init;}
    public string Host {get; init;}
    public long ContentLength {get; init;}
    public string ContentType {get; init;}
    public string QueryString {get; init;}
    public string Protocol {get; init;}
    public string UserAgent {get; init;}
    public string X_Forwarded_For {get; init;}
    public string X_Forwarded_Proto {get; init;}
    public string X_Forwarded_Host {get; init;}
    public string X_Original_For {get; init;}
    public string X_Original_Proto {get; init;}
    public string X_Original_Host {get; init;}
    public RequestLog(string method, string scheme, string pathBase, string path, string host, long? length, string ip, string queryString, string contentType, string protocol, IHeaderDictionary headers)
    {
        IP = ip;
        Method = method;
        Scheme = scheme;
        PathBase = pathBase;
        Path = path;
        Host = host;
        ContentLength = length ?? 0;
        ContentType = contentType;
        QueryString = queryString;
        Protocol = protocol;
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
    }
}
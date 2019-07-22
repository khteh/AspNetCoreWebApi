using System.Net;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Web.Api.Models.Logging
{
    public class RequestLog
    {
        [JsonProperty]
        public string Method {get; private set;}
        [JsonProperty]
        public string Scheme {get; private set;}
        [JsonProperty]
        public string PathBase {get; private set;}
        [JsonProperty]
        public string Path {get; private set;}
        [JsonProperty]
        public IPAddress IP {get; private set;}
        [JsonProperty]
        public HostString Host {get; private set;}
        [JsonProperty]
        public long ContentLength {get; private set;}
        #if false
        public RequestLog(HttpRequest request, IPAddress ip)
        {
            if (request != null)
            {
                Method = request.Method;
                Scheme = request.Scheme;
                PathBase = request.PathBase;
                Path = request.Path;
                Host = request.Host;
                ContentLength = request.ContentLength ?? 0;
            }
            IP = ip;
        }
        #endif
        public RequestLog(string method, string scheme, string pathBase, string path, HostString host, long? length, IPAddress ip)
        {
            Method = method;
            Scheme = scheme;
            PathBase = pathBase;
            Path = path;
            Host = host;
            ContentLength = length ?? 0;
        }
    }
}
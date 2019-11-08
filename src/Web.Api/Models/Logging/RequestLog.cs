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
        public string LocalIP {get; private set;}

        [JsonProperty]
        public string IP {get; private set;}
        [JsonProperty]
        public string Host {get; private set;}
        [JsonProperty]
        public long ContentLength {get; private set;}
        [JsonProperty]
        public string ContentType {get; private set;}
        [JsonProperty]
        public string QueryString {get; private set;}
        [JsonProperty]
        public string Protocol {get; private set;}
        [JsonProperty]
        public string UserAgent {get; private set;}
        [JsonProperty]
        public string X_Forwarded_For {get; private set;}
        [JsonProperty]
        public string X_Forwarded_Proto {get; private set;}
        [JsonProperty]
        public string X_Forwarded_Host {get; private set;}
        [JsonProperty]
        public string X_Original_For {get; private set;}
        [JsonProperty]
        public string X_Original_Proto {get; private set;}
        [JsonProperty]
        public string X_Original_Host {get; private set;}
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
}
// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
using System.Security.Authentication;
using Web.Api.Core.Configuration;
using Web.Api.Ping;
using static System.Console;
using static Web.Api.Ping.SvcPing;

internal class Program
{
    private static GrpcConfig _grpcConfig;
    private static async Task<int> Main(string[] args)
    {
        try
        {
            string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(contentRootFull)
                    .AddJsonFile($"appsettings.{environment}.json", false, true)
                    .AddEnvironmentVariables().Build();
            _grpcConfig = config.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            WriteLine($"Environment: {environment}, server: {_grpcConfig.Endpoint}");
            // The port number must match the port of the gRPC server.
            RootCommand rootCommand = new RootCommand("Various commands which run on shell");
            Option<string> tokenOption = new("--token") { Description = "Access Token" };
            Command ping = new Command("ping", "Ping grpc service") { tokenOption };
            rootCommand.Add(ping);
            ping.SetAction(parseResult => PingHandler(parseResult.GetValue(tokenOption)));
            return await rootCommand.Parse(args).InvokeAsync();
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
            return -1;
        }
    }
    internal static async Task PingHandler(string token)
    {
        WriteLine($"Press ENTER when the gRPC server @{_grpcConfig.Endpoint} is ready");
        ReadLine();
        try
        {
            Metadata headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");
            HttpClientHandler httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                //ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => { return true; },
                SslProtocols = SslProtocols.Tls13
            };
            // Return `true` to allow certificates that are untrusted/invalid
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var channel = GrpcChannel.ForAddress(_grpcConfig.Endpoint, new GrpcChannelOptions { HttpHandler = httpHandler });
            SvcPingClient client = new SvcPingClient(channel);
            Pong response = await client.PingAsync(new Google.Protobuf.WellKnownTypes.Empty());
            if (!string.IsNullOrEmpty(response.Message))
                WriteLine($"Ping response: {response.Message}");
            else
                WriteLine($"Failed to ping {_grpcConfig.Endpoint}!");
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
        }
    }
}
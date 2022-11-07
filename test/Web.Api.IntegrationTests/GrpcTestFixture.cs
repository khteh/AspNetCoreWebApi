namespace Web.Api.IntegrationTests;
#if false
public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
{
    private readonly WebApplicationFactory<TStartup> _factory;
    public LoggerFactory LoggerFactory { get; }
    public GrpcChannel GrpcChannel { get; }
    public GrpcTestFixture()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
        string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
        IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(contentRootFull)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddEnvironmentVariables().Build();
        GrpcConfig grpcConfig = config.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
        LoggerFactory = new LoggerFactory();
        _factory = new CustomGRPCWebApplicationFactory<TStartup>();
        var client = _factory.CreateDefaultClient(new Http3Handler());
        client.BaseAddress = new Uri(grpcConfig.Endpoint);
        GrpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
        {
            LoggerFactory = LoggerFactory,
            HttpClient = client
        });
    }
    public void Dispose()
    {
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}
#endif
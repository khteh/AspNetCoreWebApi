using Xunit;

namespace Web.Api.IntegrationTests.Services;
[CollectionDefinition("GRPC Test Collection")]
public class GrpcTestCollection : ICollectionFixture<CustomGRPCWebApplicationFactory<Program>>
{
}

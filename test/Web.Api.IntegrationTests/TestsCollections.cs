using Xunit;

namespace Web.Api.IntegrationTests.Controllers;
[CollectionDefinition("Controller Test Collection")]
public class ControllerTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
[CollectionDefinition("EmailChangeConfirmation Test Collection")]
public class EmailChangeConfirmationTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
[CollectionDefinition("EmailConfirmationCode Test Collection")]
public class EmailConfirmationCodeTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
[CollectionDefinition("SignalR Test Collection")]
public class SignalRTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

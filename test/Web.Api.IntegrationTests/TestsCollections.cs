using Xunit;

namespace Web.Api.IntegrationTests.Controllers;
[CollectionDefinition(Name)]
public class ControllerTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    /// <summary>
    /// The name of the test fixture.
    /// </summary>
    public const string Name = "Controller Test Collection";
}
[CollectionDefinition(Name)]
public class EmailChangeConfirmationTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    /// <summary>
    /// The name of the test fixture.
    /// </summary>
    public const string Name = "EmailChangeConfirmation Test Collection";
}
[CollectionDefinition(Name)]
public class EmailConfirmationCodeTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    /// <summary>
    /// The name of the test fixture.
    /// </summary>
    public const string Name = "EmailConfirmationCode Test Collection";
}
[CollectionDefinition(Name)]
public class SignalRTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    /// <summary>
    /// The name of the test fixture.
    /// </summary>
    public const string Name = "SignalR Test Collection";
}

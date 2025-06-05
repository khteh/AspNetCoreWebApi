using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;
namespace Web.Api.UnitTests.Presenters;

public class LogInPresenterUnitTests
{
    [Fact]
    public async Task Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
    {
        // arrange
        const string token = "777888AAABBB";
        var logger = new Mock<ILogger<LogInPresenter>>();
        var presenter = new LogInPresenter(logger.Object);

        // act
        await presenter.Handle(new LogInResponse(new AccessToken(token, 0), string.Empty, true));

        // assert
        LogInResponse data = JsonSerializer.DeSerializeObject<LogInResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        Assert.Equal(token, data.AccessToken.Token);
    }
    [Fact]
    public async Task Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var logger = new Mock<ILogger<LogInPresenter>>();
        var presenter = new LogInPresenter(logger.Object);

        // act
        await presenter.Handle(new LogInResponse(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

        // assert
        Models.Response.LogInResponse response = JsonSerializer.DeSerializeObject<Models.Response.LogInResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.Unauthorized, presenter.ContentResult.StatusCode);
        Assert.NotNull(response);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        Assert.False(response.Success);
        Assert.NotNull(response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Code));
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Description));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Errors.First().Code);
        Assert.Equal("Invalid username/password", response.Errors.First().Description);
    }
}
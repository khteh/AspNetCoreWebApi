using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;
namespace Web.Api.UnitTests.Presenters;
public class LogInPresenterUnitTests
{
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
    {
        // arrange
        var presenter = new LogInPresenter();

        // act
        presenter.Handle(new LogInResponse(new AccessToken("", 0), "", true));

        // assert
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
    }
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
    {
        // arrange
        const string token = "777888AAABBB";
        var presenter = new LogInPresenter();

        // act
        presenter.Handle(new LogInResponse(new AccessToken(token, 0), "", true));

        // assert
        LogInResponse data = JsonSerializer.DeSerializeObject<LogInResponse>(presenter.ContentResult.Content);
        Assert.Equal(token, data.AccessToken.Token);
    }
    [Fact]
    public void Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var presenter = new LogInPresenter();

        // act
        presenter.Handle(new LogInResponse(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

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
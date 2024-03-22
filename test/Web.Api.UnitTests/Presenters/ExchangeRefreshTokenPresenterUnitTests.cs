using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.DTO;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;
namespace Web.Api.UnitTests.Presenters;
public class ExchangeRefreshTokenPresenterUnitTests
{
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
    {
        // arrange
        const string token = "777888AAABBB";
        var logger = new Mock<ILogger<ExchangeRefreshTokenPresenter>>();
        var presenter = new ExchangeRefreshTokenPresenter(logger.Object);

        // act
        presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new AccessToken(token, 0), "", true));

        // assert
        Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse data = JsonSerializer.DeSerializeObject<Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse>(presenter.ContentResult.Content);
        Assert.NotNull(data);
        Assert.NotNull(data.AccessToken);
        Assert.False(string.IsNullOrEmpty(data.AccessToken.Token));
        Assert.Equal(token, data.AccessToken.Token);
    }
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsRefreshToken()
    {
        // arrange
        const string token = "777888AAABBB";
        var logger = new Mock<ILogger<ExchangeRefreshTokenPresenter>>();
        var presenter = new ExchangeRefreshTokenPresenter(logger.Object);

        // act
        presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(null, token, true));

        // assert
        Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse data = JsonSerializer.DeSerializeObject<Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse>(presenter.ContentResult.Content);
        Assert.Equal(token, data.RefreshToken);
    }
    [Fact]
    public void Handle_GivenFailedUseCaseResponse_SetsError()
    {
        // arrange
        var logger = new Mock<ILogger<ExchangeRefreshTokenPresenter>>();
        var presenter = new ExchangeRefreshTokenPresenter(logger.Object);

        // act
        presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid Token!") }));

        // assert
        Models.Response.ExchangeRefreshTokenResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ExchangeRefreshTokenResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
        Assert.NotNull(response);
        Assert.Null(response.AccessToken);
        Assert.True(string.IsNullOrEmpty(response.RefreshToken));
        Assert.NotNull(response.Errors);
        Assert.Single(response.Errors);
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Errors.First().Code);
        Assert.Equal("Invalid Token!", response.Errors.First().Description);
    }
}

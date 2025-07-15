using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.DTO;
using Web.Api.Presenters.Grpc;
using Xunit;
namespace Web.Api.UnitTests.Presenters.Grpc;

public class GRPCExchangeRefreshTokenPresenterUnitTests
{
    private readonly IMapper _mapper;
    public GRPCExchangeRefreshTokenPresenterUnitTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(GrpcProfile).Assembly));
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
    {
        // arrange
        const string token = "777888AAABBB";
        var presenter = new ExchangeRefreshTokenPresenter(_mapper);

        // act
        presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new AccessToken(token, 0), "", true));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.NotNull(presenter.Response.Response);
        Assert.True(presenter.Response.Response.Success);
        Assert.False(presenter.Response.Response.Errors.Any());
        Assert.NotNull(presenter.Response.AccessToken);
        Assert.Equal(token, presenter.Response.AccessToken.Token);
    }
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsRefreshToken()
    {
        // arrange
        const string token = "777888AAABBB";
        var presenter = new ExchangeRefreshTokenPresenter(_mapper);

        // act
        presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(null, token, true));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.True(presenter.Response.Response.Success);
        Assert.False(presenter.Response.Response.Errors.Any());
        Assert.False(string.IsNullOrEmpty(presenter.Response.RefreshToken));
        Assert.Equal(token, presenter.Response.RefreshToken);
    }
    [Fact]
    public void Handle_GivenFailedUseCaseResponse_SetsError()
    {
        // arrange
        var presenter = new ExchangeRefreshTokenPresenter(_mapper);

        // act
        presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid Token!") }));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.Null(presenter.Response.AccessToken);
        Assert.True(string.IsNullOrEmpty(presenter.Response.RefreshToken));
        Assert.NotNull(presenter.Response.Response.Errors);
        Assert.Single(presenter.Response.Response.Errors);
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), presenter.Response.Response.Errors.First().Code);
        Assert.Equal("Invalid Token!", presenter.Response.Response.Errors.First().Description);
    }
}
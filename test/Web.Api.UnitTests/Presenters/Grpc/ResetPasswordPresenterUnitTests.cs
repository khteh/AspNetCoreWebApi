using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces;
using Web.Api.Presenters.Grpc;
using Xunit;
namespace Web.Api.UnitTests.Presenters.Grpc;
public class GRPCResetPasswordPresenterUnitTests
{
    private readonly IMapper _mapper;
    public GRPCResetPasswordPresenterUnitTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddAutoMapper(typeof(GrpcProfile));
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
    {
        // arrange
        var presenter = new ResetPasswordPresenter(_mapper);

        // act
        presenter.Handle(new UseCaseResponseMessage("", true));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.True(presenter.Response.Success);
        //Assert.Null(presenter.Response.Errors);
        Assert.False(presenter.Response.Errors.Any());
    }
    [Fact]
    public void Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var presenter = new ResetPasswordPresenter(_mapper);

        // act
        presenter.Handle(new UseCaseResponseMessage(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.NotNull(presenter.Response.Errors);
        Assert.NotEmpty(presenter.Response.Errors);
        Assert.False(string.IsNullOrEmpty(presenter.Response.Errors.First().Code));
        Assert.False(string.IsNullOrEmpty(presenter.Response.Errors.First().Description));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), presenter.Response.Errors.First().Code);
        Assert.Equal("Invalid username/password", presenter.Response.Errors.First().Description);
    }
}
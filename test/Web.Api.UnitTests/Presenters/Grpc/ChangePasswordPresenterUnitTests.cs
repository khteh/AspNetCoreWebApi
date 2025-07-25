using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces;
using Web.Api.Presenters.Grpc;
using Xunit;
namespace Web.Api.UnitTests.Presenters.Grpc;

public class GRPCChangePasswordPresenterUnitTests
{
    private readonly IMapper _mapper;
    public GRPCChangePasswordPresenterUnitTests()
    {
        IServiceCollection services = new ServiceCollection();
        // Add logging services, which includes registering ILoggerFactory
        services.AddLogging(builder => builder.AddConsole());
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(GrpcProfile).Assembly));
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
    [Fact]
    public async Task Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
    {
        // arrange
        var presenter = new ChangePasswordPresenter(_mapper);

        // act
        await presenter.Handle(new UseCaseResponseMessage("", true));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.True(presenter.Response.Success);
        Assert.False(presenter.Response.Errors.Any());
    }
    [Fact]
    public async Task Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var presenter = new ChangePasswordPresenter(_mapper);

        // act
        await presenter.Handle(new UseCaseResponseMessage(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

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
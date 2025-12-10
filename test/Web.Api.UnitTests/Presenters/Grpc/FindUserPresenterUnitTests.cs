using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters.Grpc;
using Xunit;
namespace Web.Api.UnitTests.Presenters.Grpc;

public class GRPCFindUserPresenterUnitTests
{
    private readonly IMapper _mapper;
    public GRPCFindUserPresenterUnitTests()
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
        var presenter = new FindUserPresenter(_mapper);

        // act
        await presenter.Handle(new FindUserResponse(new User(), Guid.Empty, true, null, null));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.NotNull(presenter.Response.Response);
        Assert.True(presenter.Response.Response.Success);
        Assert.False(presenter.Response.Response.Errors.Any());
    }
    [Fact]
    public async Task Handle_GivenSuccessfulUseCaseResponse_SetsId()
    {
        // arrange
        var presenter = new FindUserPresenter(_mapper);

        // act
        Guid id = Guid.CreateVersion7();
        await presenter.Handle(new FindUserResponse(new User(), id, true));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.NotNull(presenter.Response.Response);
        Assert.True(presenter.Response.Response.Success);
        Assert.False(presenter.Response.Response.Errors.Any());
        Assert.Equal(id.ToString(), presenter.Response.Id);
    }
    [Fact]
    public async Task Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var presenter = new FindUserPresenter(_mapper);

        // act
        await presenter.Handle(new FindUserResponse(null, Guid.Empty, false, null, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

        // assert
        Assert.NotNull(presenter.Response);
        Assert.NotNull(presenter.Response.Response.Errors);
        Assert.NotEmpty(presenter.Response.Response.Errors);
        Assert.Equal("missing first name", presenter.Response.Response.Errors.First().Description);
        Assert.False(presenter.Response.Response.Success);
        Assert.NotNull(presenter.Response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Code));
        Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Description));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), presenter.Response.Response.Errors.First().Code);
    }
}
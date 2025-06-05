using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;
namespace Web.Api.UnitTests.Presenters;

public class LockUserPresenterUnitTests
{
    [Fact]
    public async Task Handle_GivenSuccessfulUseCaseResponse_SetsId()
    {
        // arrange
        var logger = new Mock<ILogger<LockUserPresenter>>();
        var presenter = new LockUserPresenter(logger.Object);

        // act
        Guid id = Guid.CreateVersion7();
        await presenter.Handle(new UseCaseResponseMessage(id.ToString(), true));

        // assert
        LockUserResponse data = JsonSerializer.DeSerializeObject<LockUserResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        Assert.True(data.Success);
    }
    [Fact]
    public async Task Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var logger = new Mock<ILogger<LockUserPresenter>>();
        var presenter = new LockUserPresenter(logger.Object);

        // act
        await presenter.Handle(new UseCaseResponseMessage(null, false, null, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

        // assert
        Models.Response.LockUserResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LockUserResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
        Assert.NotNull(response);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("missing first name", response.Errors.First().Description);
        Assert.False(response.Success);
        Assert.NotNull(response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Code));
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Description));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Errors.First().Code);
    }
}
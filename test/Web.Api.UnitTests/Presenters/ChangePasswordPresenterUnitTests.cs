using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Presenters;
using Xunit;
namespace Web.Api.UnitTests.Presenters;

public class ChangePasswordPresenterUnitTests
{
    [Fact]
    public async Task Handle_GivenSuccessfulUseCaseResponse_SetsId()
    {
        // arrange
        var logger = new Mock<ILogger<ChangePasswordPresenter>>();
        var presenter = new ChangePasswordPresenter(logger.Object);

        // act
        Guid id = Guid.CreateVersion7();
        await presenter.Handle(new UseCaseResponseMessage(id, true));

        // assert
        ChangePasswordResponse response = Serialization.JsonSerializer.DeSerializeObject<ChangePasswordResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        Assert.NotNull(response);
        Assert.Empty(response.Errors);
        Assert.True(response.Success);
    }
    [Fact]
    public async Task Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var logger = new Mock<ILogger<ChangePasswordPresenter>>();
        var presenter = new ChangePasswordPresenter(logger.Object);

        // act
        await presenter.Handle(new UseCaseResponseMessage(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

        // assert
        ChangePasswordResponse response = Serialization.JsonSerializer.DeSerializeObject<ChangePasswordResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
        Assert.NotNull(response);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Code));
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Description));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Errors.First().Code);
        Assert.Equal("Invalid username/password", response.Errors.First().Description);
    }
}
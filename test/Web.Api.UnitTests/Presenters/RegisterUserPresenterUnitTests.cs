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
using Web.Api.Serialization;
using Xunit;
namespace Web.Api.UnitTests.Presenters;

public class RegisterUserPresenterUnitTests
{
    [Fact]
    public async Task Handle_GivenSuccessfulUseCaseResponse_SetsId()
    {
        // arrange
        var logger = new Mock<ILogger<RegisterUserPresenter>>();
        var presenter = new RegisterUserPresenter(logger.Object);

        // act
        Guid id = Guid.CreateVersion7();
        await presenter.Handle(new UseCaseResponseMessage(id, true));

        // assert
        RegisterUserResponse data = JsonSerializer.DeSerializeObject<RegisterUserResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.Created, presenter.ContentResult.StatusCode);
        Assert.True(data.Success);
        Assert.Equal(id, Guid.Parse(data.Id));
    }
    [Fact]
    public async Task Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var logger = new Mock<ILogger<RegisterUserPresenter>>();
        var presenter = new RegisterUserPresenter(logger.Object);

        // act
        await presenter.Handle(new UseCaseResponseMessage(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

        // assert
        RegisterUserResponse response = Serialization.JsonSerializer.DeSerializeObject<RegisterUserResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
        Assert.NotNull(response);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Code));
        Assert.False(string.IsNullOrEmpty(response.Errors.First().Description));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Errors.First().Code);
        Assert.Equal("missing first name", response.Errors.First().Description);
    }
}
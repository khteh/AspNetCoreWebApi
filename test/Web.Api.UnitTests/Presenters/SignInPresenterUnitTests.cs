using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;
namespace Web.Api.UnitTests.Presenters;
public class SignInPresenterUnitTests
{
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
    {
        // arrange
        var logger = new Mock<ILogger<SignInPresenter>>();
        var presenter = new SignInPresenter(logger.Object);

        // act
        Guid id = Guid.NewGuid();
        string username = "UserName";
        presenter.Handle(new SignInResponse(id, username, true, string.Empty));

        // assert
        Models.Response.SignInResponse data = JsonSerializer.DeSerializeObject<Models.Response.SignInResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        Assert.True(data.Success);
    }
    [Fact]
    public void Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var logger = new Mock<ILogger<SignInPresenter>>();
        var presenter = new SignInPresenter(logger.Object);

        // act
        presenter.Handle(new SignInResponse(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

        // assert
        Models.Response.SignInResponse response = JsonSerializer.DeSerializeObject<Models.Response.SignInResponse>(presenter.ContentResult.Content);
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
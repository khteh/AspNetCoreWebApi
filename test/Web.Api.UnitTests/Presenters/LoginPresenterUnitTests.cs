﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;
namespace Web.Api.UnitTests.Presenters;
public class LoginPresenterUnitTests
{
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
    {
        // arrange
        var presenter = new LoginPresenter();

        // act
        presenter.Handle(new LoginResponse(new AccessToken("", 0), "", true));

        // assert
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
    }
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
    {
        // arrange
        const string token = "777888AAABBB";
        var presenter = new LoginPresenter();

        // act
        presenter.Handle(new LoginResponse(new AccessToken(token, 0), "", true));

        // assert
        LoginResponse data = JsonSerializer.DeSerializeObject<LoginResponse>(presenter.ContentResult.Content);
        Assert.Equal(token, data.AccessToken.Token);
    }
    [Fact]
    public void Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var presenter = new LoginPresenter();

        // act
        presenter.Handle(new LoginResponse(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

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
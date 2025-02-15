using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Presenters;
using Xunit;
namespace Web.Api.UnitTests.Presenters;
public class CodePresenterUnitTests
{
    [Fact]
    public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
    {
        // arrange
        var logger = new Mock<ILogger<CodePresenter>>();
        var presenter = new CodePresenter(logger.Object);

        // act
        Guid id = Guid.CreateVersion7(TimeProvider.System.GetUtcNow());
        string code = "1234";
        presenter.Handle(new Core.DTO.UseCaseResponses.CodeResponse(id.ToString(), code, true));

        // assert
        CodeResponse response = Serialization.JsonSerializer.DeSerializeObject<CodeResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        Assert.NotNull(response);
        Assert.Equal(id, response.Id);
        Assert.False(string.IsNullOrEmpty(response.Code));
        Assert.Equal(code, response.Code);
        Assert.Null(response.Errors);
        Assert.True(response.Success);
    }
    [Fact]
    public void Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var logger = new Mock<ILogger<CodePresenter>>();
        var presenter = new CodePresenter(logger.Object);

        // act
        presenter.Handle(new Core.DTO.UseCaseResponses.CodeResponse(string.Empty, string.Empty, false, "Invalid username/password", new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

        // assert
        CodeResponse response = Serialization.JsonSerializer.DeSerializeObject<CodeResponse>(presenter.ContentResult.Content);
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
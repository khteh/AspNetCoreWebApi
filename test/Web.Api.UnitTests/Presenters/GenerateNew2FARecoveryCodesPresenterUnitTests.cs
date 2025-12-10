using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Web.Api.Models.Response;
using Xunit;
using System.Threading.Tasks;
namespace Web.Api.UnitTests.Presenters;

public class GenerateNew2FARecoveryCodesPresenterUnitTests
{
    [Fact]
    public async Task Handle_GivenSuccessfulUseCaseResponse_SetsId()
    {
        // arrange
        var logger = new Mock<ILogger<GenerateNew2FARecoveryCodesPresenter>>();
        var presenter = new GenerateNew2FARecoveryCodesPresenter(logger.Object);

        // act
        Guid id = Guid.CreateVersion7();
        List<string> codes = new List<string>() { "123" };
        await presenter.Handle(new Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse(id, codes, true));

        // assert
        Models.Response.GenerateNew2FARecoveryCodesResponse data = JsonSerializer.DeSerializeObject<Models.Response.GenerateNew2FARecoveryCodesResponse>(presenter.ContentResult.Content);
        Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        Assert.True(data.Success);
        Assert.Equal(codes, data.Codes);
    }
    [Fact]
    public async Task Handle_GivenFailedUseCaseResponse_SetsErrors()
    {
        // arrange
        var logger = new Mock<ILogger<GenerateNew2FARecoveryCodesPresenter>>();
        var presenter = new GenerateNew2FARecoveryCodesPresenter(logger.Object);

        // act
        await presenter.Handle(new Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse(Guid.Empty, null, false, null, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

        // assert
        Models.Response.GenerateNew2FARecoveryCodesResponse response = JsonSerializer.DeSerializeObject<Models.Response.GenerateNew2FARecoveryCodesResponse>(presenter.ContentResult.Content);
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
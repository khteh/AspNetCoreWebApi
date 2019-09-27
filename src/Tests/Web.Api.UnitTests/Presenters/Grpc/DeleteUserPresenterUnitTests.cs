using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Presenters.Grpc;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class GRPCDeleteUserPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("", true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid user!") }));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response.Errors);
            Assert.NotEmpty(presenter.Response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Code));
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Description));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), presenter.Response.Response.Errors.First().Code);
            Assert.Equal("Invalid user!", presenter.Response.Response.Errors.First().Description);
        }
    }
}
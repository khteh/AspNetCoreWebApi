using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Presenters;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class ChangePasswordPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new ChangePasswordPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("", true));

            // assert
            Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new ChangePasswordPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("1234", true));

            // assert
            ChangePasswordResponse response = Serialization.JsonSerializer.DeSerializeObject<ChangePasswordResponse>(presenter.ContentResult.Content);
            Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
            Assert.NotNull(response);
            Assert.Null(response.Errors);
            Assert.True(response.Success);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new ChangePasswordPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

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
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class RegisterUserPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new RegisterUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("", true));

            // assert
            Assert.Equal((int)HttpStatusCode.Created, presenter.ContentResult.StatusCode);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new RegisterUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage("1234", true));

            // assert
            UseCaseResponseMessage data = JsonSerializer.DeSerializeObject<UseCaseResponseMessage>(presenter.ContentResult.Content);
            Assert.True(data.Success);
            Assert.Equal("1234", data.Id);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new RegisterUserPresenter();

            // act
            presenter.Handle(new UseCaseResponseMessage(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

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
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Core.Dto;
using Web.Api.Core.Dto.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
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
            presenter.Handle(new ChangePasswordResponse("", true));

            // assert
            Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new ChangePasswordPresenter();

            // act
            presenter.Handle(new ChangePasswordResponse("1234", true));

            // assert
            dynamic data = JsonConvert.DeserializeObject(presenter.ContentResult.Content);
            Assert.True(data.success.Value);
            Assert.Equal("1234", data.id.Value);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new ChangePasswordPresenter();

            // act
            presenter.Handle(new ChangePasswordResponse(new List<Error> { new Error("", "Invalid username/password") }));

            // assert
            ChangePasswordResponse data = Serialization.JsonSerializer.DeSerializeObject<ChangePasswordResponse>(presenter.ContentResult.Content);
            Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
            Assert.Null(data.Errors);
            Assert.NotEmpty(data.Errors);
            Assert.Equal("Invalid username/password", data.Errors.First().Description);
        }
    }
}
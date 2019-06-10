using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
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
            List<Error> errors = Serialization.JsonSerializer.DeSerializeObject<List<Error>>(presenter.ContentResult.Content);
            Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
            Assert.NotNull(errors);
            Assert.NotEmpty(errors);
            Assert.Equal("Invalid username/password", errors.First().Description);
        }
    }
}
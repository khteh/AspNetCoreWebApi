using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Core.Dto;
using Web.Api.Core.Dto.UseCaseResponses;
using Web.Api.Presenters;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class DeleteUserPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new DeleteUserResponse("", true));

            // assert
            Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new DeleteUserResponse("1234", true));

            // assert
            dynamic data = JsonConvert.DeserializeObject(presenter.ContentResult.Content);
            Assert.True(data.success.Value);
            Assert.Equal("1234", data.id.Value);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new DeleteUserPresenter();

            // act
            presenter.Handle(new DeleteUserResponse(new List<Error>() { new Error(null, "Invalid user!") }));

            // assert
            DeleteUserResponse data = Serialization.JsonSerializer.DeSerializeObject<DeleteUserResponse>(presenter.ContentResult.Content);
            Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
            Assert.NotNull(data.Errors);
            Assert.NotEmpty(data.Errors);
            Assert.Equal("Invalid user!", data.Errors.First().Description);
        }
    }
}
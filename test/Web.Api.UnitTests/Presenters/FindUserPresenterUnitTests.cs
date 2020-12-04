using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class FindUserPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new FindUserPresenter();

            // act
            presenter.Handle(new FindUserResponse(new User(), "", true, null, null));

            // assert
            Assert.Equal((int)HttpStatusCode.OK, presenter.ContentResult.StatusCode);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new FindUserPresenter();

            // act
            presenter.Handle(new FindUserResponse(new User(), "1234", true));

            // assert
            FindUserResponse data = JsonSerializer.DeSerializeObject<FindUserResponse>(presenter.ContentResult.Content);
            Assert.True(data.Success);
            Assert.Equal("1234", data.Id);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new FindUserPresenter();

            // act
            presenter.Handle(new FindUserResponse(null, null, false, null, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

            // assert
            Models.Response.FindUserResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.FindUserResponse>(presenter.ContentResult.Content);
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
}
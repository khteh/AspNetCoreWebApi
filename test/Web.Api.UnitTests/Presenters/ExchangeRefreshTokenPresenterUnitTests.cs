using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Models.Response;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Xunit;

namespace Web.Api.UnitTests.Presenters
{
    public class ExchangeRefreshTokenPresenterUnitTests
    {
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
        {
            // arrange
            const string token = "777888AAABBB";
            var presenter = new ExchangeRefreshTokenPresenter();

            // act
            presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new AccessToken(token, 0), "", true));

            // assert
            dynamic data = JsonConvert.DeserializeObject(presenter.ContentResult.Content);
            Assert.Equal(token, data.accessToken.token.Value);
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsRefreshToken()
        {
            // arrange
            const string token = "777888AAABBB";
            var presenter = new ExchangeRefreshTokenPresenter();

            // act
            presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(null, token, true));

            // assert
            dynamic data = JsonConvert.DeserializeObject(presenter.ContentResult.Content);
            Assert.Equal(token, data.refreshToken.Value);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsError()
        {
            // arrange
            var presenter = new ExchangeRefreshTokenPresenter();

            // act
            presenter.Handle(new Core.DTO.UseCaseResponses.ExchangeRefreshTokenResponse(new List<Error>() { new Error("InvalidToken", "Invalid Token!")}));

            // assert
            Models.Response.ExchangeRefreshTokenResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ExchangeRefreshTokenResponse>(presenter.ContentResult.Content);
            Assert.Equal((int)HttpStatusCode.BadRequest, presenter.ContentResult.StatusCode);
            Assert.NotNull(response);
            Assert.Null(response.AccessToken);
            Assert.True(string.IsNullOrEmpty(response.RefreshToken));
            Assert.NotNull(response.Errors);
            Assert.Single(response.Errors);
            Assert.Equal("InvalidToken", response.Errors.First().Code);
            Assert.Equal("Invalid Token!", response.Errors.First().Description);
        }
    }
}

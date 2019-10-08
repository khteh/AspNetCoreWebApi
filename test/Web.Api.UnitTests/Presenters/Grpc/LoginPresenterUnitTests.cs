using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters.Grpc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using AutoMapper;
namespace Web.Api.UnitTests.Presenters.Grpc
{
    public class GRPCLoginPresenterUnitTests
    {
        private readonly IMapper _mapper;
        public GRPCLoginPresenterUnitTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(typeof(GrpcProfile));
            _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
        }
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new LoginPresenter(_mapper);

            // act
            presenter.Handle(new LoginResponse(new AccessToken("", 0),"", true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsAccessToken()
        {
            // arrange
            const string token = "777888AAABBB";
            var presenter = new LoginPresenter(_mapper);

            // act
            presenter.Handle(new LoginResponse(new AccessToken(token, 0),"", true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
            Assert.Equal(token, presenter.Response.AccessToken.Token);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new LoginPresenter(_mapper);

            // act
            presenter.Handle(new LoginResponse(new List<Error> { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username/password") }));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response.Errors);
            Assert.NotEmpty(presenter.Response.Response.Errors);
            Assert.False(presenter.Response.Response.Success);
            Assert.NotNull(presenter.Response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Code));
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Description));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), presenter.Response.Response.Errors.First().Code);
            Assert.Equal("Invalid username/password", presenter.Response.Response.Errors.First().Description);
        }
    }
}
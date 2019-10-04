using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Presenters.Grpc;
using Web.Api.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using AutoMapper;
namespace Web.Api.UnitTests.Presenters.Grpc
{
    public class GRPCFindUserPresenterUnitTests
    {
        private readonly IMapper _mapper;
        public GRPCFindUserPresenterUnitTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(typeof(GrpcProfile));
            _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
        }
        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsOKHttpStatusCode()
        {
            // arrange
            var presenter = new FindUserPresenter(_mapper);

            // act
            presenter.Handle(new FindUserResponse(new User(), "", true, null, null));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
        }

        [Fact]
        public void Handle_GivenSuccessfulUseCaseResponse_SetsId()
        {
            // arrange
            var presenter = new FindUserPresenter(_mapper);

            // act
            presenter.Handle(new FindUserResponse(new User(), "1234", true));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response);
            Assert.True(presenter.Response.Response.Success);
            Assert.False(presenter.Response.Response.Errors.Any());
            Assert.Equal("1234", presenter.Response.Id);
        }

        [Fact]
        public void Handle_GivenFailedUseCaseResponse_SetsErrors()
        {
            // arrange
            var presenter = new FindUserPresenter(_mapper);

            // act
            presenter.Handle(new FindUserResponse(null, null, false, null, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "missing first name") }));

            // assert
            Assert.NotNull(presenter.Response);
            Assert.NotNull(presenter.Response.Response.Errors);
            Assert.NotEmpty(presenter.Response.Response.Errors);
            Assert.Equal("missing first name", presenter.Response.Response.Errors.First().Description);
            Assert.False(presenter.Response.Response.Success);
            Assert.NotNull(presenter.Response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Code));
            Assert.False(string.IsNullOrEmpty(presenter.Response.Response.Errors.First().Description));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), presenter.Response.Response.Errors.First().Code);
        }
    }
}
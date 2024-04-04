using AutoMapper;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
using Web.Api.Infrastructure.Interfaces;
using Xunit;
using Xunit.Abstractions;
namespace Web.Api.Infrastructure.UnitTests.UserRepository;
public class InfrastructureTestBase : IDisposable
{
    public Mock<UserManager<AppUser>> UserManager { get; } = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
    public Mock<SignInManager<AppUser>> SignInManager { get; }
    public Mock<ILogger<Data.Repositories.UserRepository>> Logger { get; } = new Mock<ILogger<Data.Repositories.UserRepository>>();
    public Mock<IMapper> Mapper { get; } = new Mock<IMapper>();
    public Mock<AppDbContext> AppDbContext { get; } = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
    public Mock<IJwtFactory> JwtFactory { get; } = new Mock<IJwtFactory>();
    public Mock<IJwtTokenValidator> JwtTokenValidator { get; } = new Mock<IJwtTokenValidator>();
    public Data.Repositories.UserRepository UserRepository { get; private set; }
    public Mock<ITokenFactory> TokenFactory { get; } = new Mock<ITokenFactory>();
    public InfrastructureTestBase()
    {
        SignInManager = new Mock<SignInManager<AppUser>>(UserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);
        SignInManager.Setup(i => i.RefreshSignInAsync(It.IsAny<AppUser>()));
        Mapper.Setup(i => i.Map<AppUser, User>(It.IsAny<AppUser>(), It.IsAny<User>())).Returns(new User());
        UserRepository = new Data.Repositories.UserRepository(Logger.Object, UserManager.Object, SignInManager.Object, Mapper.Object, AppDbContext.Object, JwtFactory.Object, JwtTokenValidator.Object, TokenFactory.Object);
    }
    public void Dispose()
    {
        UserRepository.Dispose();
        GC.SuppressFinalize(this);
    }
}
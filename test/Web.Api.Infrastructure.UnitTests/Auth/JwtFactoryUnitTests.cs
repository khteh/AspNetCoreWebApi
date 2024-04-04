using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Interfaces;
using Xunit;
namespace Web.Api.Infrastructure.UnitTests.Auth;
public class JwtFactoryUnitTests
{
    [Fact]
    public async void GenerateEncodedToken_GivenValidInputs_ReturnsExpectedTokenData()
    {
        // arrange
        var token = Guid.NewGuid().ToString();
        var id = Guid.NewGuid().ToString();
        var jwtIssuerOptions = new JwtIssuerOptions
        {
            Issuer = "",
            Audience = "",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secret_key")), SecurityAlgorithms.HmacSha512)
        };
        Mock<ILogger<IJwtFactory>> logger = new Mock<ILogger<IJwtFactory>>();
        var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
        mockJwtTokenHandler.Setup(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>())).Returns(token);

        var jwtFactory = new JwtFactory(logger.Object, mockJwtTokenHandler.Object, Options.Create(jwtIssuerOptions));

        // act
        var result = await jwtFactory.GenerateEncodedToken(id, "userName");

        // assert
        Assert.Equal(token, result.Token);
        mockJwtTokenHandler.VerifyAll();
    }
    [Fact]
    public async void GenerateEncodedToken_GivenInValidInputs_ReturnsNullTokenData()
    {
        // arrange
        var token = Guid.NewGuid().ToString();
        var id = Guid.NewGuid().ToString();
        var jwtIssuerOptions = new JwtIssuerOptions
        {
            Issuer = "",
            Audience = "",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secret_key")), SecurityAlgorithms.HmacSha512)
        };
        Mock<ILogger<IJwtFactory>> logger = new Mock<ILogger<IJwtFactory>>();
        var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
        mockJwtTokenHandler.Setup(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>())).Returns(token);

        var jwtFactory = new JwtFactory(logger.Object, mockJwtTokenHandler.Object, Options.Create(jwtIssuerOptions));

        // act
        var result = await jwtFactory.GenerateEncodedToken(id, string.Empty);

        // assert
        Assert.Null(result);
        mockJwtTokenHandler.Verify(i => i.WriteToken(It.IsAny<JwtSecurityToken>()), Times.Never);
    }
}
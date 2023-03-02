using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Web.Api.Helpers;
using Xunit;
using Web.Api.Core.Helpers;

namespace Web.Api.UnitTests.Helpers;
public class EmailValidationUnitTests
{
    [Fact]
    public void ValidEmailShouldPassTests()
    {
        Assert.True(EmailValidation.IsValidEmail("me@email.com"));
        Assert.True(EmailValidation.IsValidEmail("1@2.3"));
        Assert.True(EmailValidation.IsValidEmail(".@2.3"));
        Assert.True(EmailValidation.IsValidEmail("me@email.com.sg"));
        Assert.True(EmailValidation.IsValidEmail("me@email.com.sg.us"));
        Assert.True(EmailValidation.IsValidEmail("me@email.com.sg.us.uk"));
        Assert.True(EmailValidation.IsValidEmail("me@email.com.sg.us.uk.de"));
        Assert.True(EmailValidation.IsValidEmail("1!#$[%^.&*()_+~]-@2.3"));
    }
    [Fact]
    public void InvalidEmailShouldFailTests()
    {
        Assert.False(EmailValidation.IsValidEmail(""));
        Assert.False(EmailValidation.IsValidEmail("1"));
        Assert.False(EmailValidation.IsValidEmail("1@2"));
        Assert.False(EmailValidation.IsValidEmail("@2.3"));
        Assert.False(EmailValidation.IsValidEmail("1@2@.3"));
        Assert.False(EmailValidation.IsValidEmail("me@email"));
        Assert.False(EmailValidation.IsValidEmail("email.com"));
    }
}
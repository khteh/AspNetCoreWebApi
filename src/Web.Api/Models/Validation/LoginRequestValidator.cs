using FluentValidation;
using Web.Api.Models.Request;
namespace Web.Api.Models.Validation;
public class LoginRequestValidator : AbstractValidator<LogInRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
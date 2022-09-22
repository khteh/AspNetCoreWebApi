using FluentValidation;
using Web.Api.Models.Request;
namespace Web.Api.Models.Validation;
public class LogInRequestValidator : AbstractValidator<LogInRequest>
{
    public LogInRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
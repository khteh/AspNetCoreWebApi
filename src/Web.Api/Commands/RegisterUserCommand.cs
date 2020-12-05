using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands
{
    public record RegisterUserCommand(string FirstName, string LastName, string Email, string UserName, string Password) : IRequest<RegisterUserResponse>;
}
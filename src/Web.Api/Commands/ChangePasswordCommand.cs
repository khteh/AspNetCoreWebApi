using MediatR;
using Web.Api.Models.Response;

namespace Web.Api.Commands
{
    public record ChangePasswordCommand(string IdentityId, string OldPassword, string NewPassword) : IRequest<ChangePasswordResponse>;
}
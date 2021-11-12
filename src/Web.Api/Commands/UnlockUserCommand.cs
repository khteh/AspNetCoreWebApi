using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record UnlockUserCommand(string Id) : IRequest<LockUserResponse>;
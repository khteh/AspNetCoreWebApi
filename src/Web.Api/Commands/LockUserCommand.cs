using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record LockUserCommand(string Id) : IRequest<LockUserResponse>;
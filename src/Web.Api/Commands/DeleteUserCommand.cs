using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record DeleteUserCommand(string UserName) : IRequest<DeleteUserResponse>;
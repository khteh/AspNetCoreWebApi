using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record LoginCommand(string UserName, string Password, string RemoteIpAddress) : IRequest<LoginResponse>;
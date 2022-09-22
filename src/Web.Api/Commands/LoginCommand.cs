using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record LogInCommand(string UserName, string Password, string RemoteIpAddress) : IRequest<LogInResponse>;
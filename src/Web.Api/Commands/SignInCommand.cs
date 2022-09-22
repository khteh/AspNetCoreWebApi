using Web.Api.Models.Response;
namespace Web.Api.Commands;

public record SignInCommand(string UserName, string Password, string RemoteIpAddress) : IRequest<LogInResponse>;
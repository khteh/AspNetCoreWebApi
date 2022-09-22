using Web.Api.Models.Response;
namespace Web.Api.Commands;

public record SignInCommand(string UserName, string Password, string RemoteIpAddress, bool rememberMe, bool lockOutOnFailure, bool isMobile) : IRequest<SignInResponse>;
using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record ResetPasswordCommand(string Id, string Email, string NewPassword, string Code, bool IsFirstLogin = false) : IRequest<ResetPasswordResponse>;
using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record ResetPasswordCommand(string Id, string NewPassword, bool IsFirstLogin = false) : IRequest<ResetPasswordResponse>;
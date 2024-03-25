using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record GenerateChangeEmailTokenCommand(string IdentityId, string Email) : IRequest<CodeResponse>;
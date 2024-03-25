using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record ConfirmEmailChangeCommand(string IdentityId, string Email, string Code) : IRequest<ResponseBase>;
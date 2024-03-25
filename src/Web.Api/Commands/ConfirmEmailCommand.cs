using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record ConfirmEmailCommand(string IdentityId, string Code) : IRequest<ResponseBase>;
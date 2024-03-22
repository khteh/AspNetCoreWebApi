using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;

public record GenerateNew2FARecoveryCodesCommand(string Id, int Codes) : IRequest<GenerateNew2FARecoveryCodesResponse>;
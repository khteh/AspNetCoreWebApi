using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
// The signing key is read from AuthSettings by the handler; it is never carried in (or logged with) the command.
public record ExchangeRefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<ExchangeRefreshTokenResponse>;
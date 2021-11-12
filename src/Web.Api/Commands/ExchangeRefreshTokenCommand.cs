using MediatR;
using Web.Api.Models.Response;
namespace Web.Api.Commands;
public record ExchangeRefreshTokenCommand(string AccessToken, string RefreshToken, string SigningKey) : IRequest<ExchangeRefreshTokenResponse>;
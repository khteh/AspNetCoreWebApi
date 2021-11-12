using System.Collections.Generic;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record ExchangeRefreshTokenResponse(AccessToken AccessToken, string RefreshToken, bool Success, List<Error> Errors) : ResponseBase(Success, Errors);
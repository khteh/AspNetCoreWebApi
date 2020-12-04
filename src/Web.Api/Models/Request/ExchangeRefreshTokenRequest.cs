namespace Web.Api.Models.Request
{
    public record ExchangeRefreshTokenRequest(string AccessToken, string RefreshToken);
}
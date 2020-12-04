namespace Web.Api.Models.Request
{
    public record ExchangeRefreshTokenRequest
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
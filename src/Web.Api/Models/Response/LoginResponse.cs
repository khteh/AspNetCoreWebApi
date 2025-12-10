using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;

public record LogInResponse : ResponseBase//(AccessToken AccessToken, string RefreshToken, bool Success, List<Error> Errors) : ResponseBase(Success, Errors);
{
    public AccessToken? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public LogInResponse(bool success, List<Error> errors) : base(success, errors) { }
    [JsonConstructor]
    public LogInResponse(AccessToken? accessToken, string? refreshToken, bool success, List<Error> errors) : base(success, errors) => (AccessToken, RefreshToken) = (accessToken, refreshToken);
}
using System.Text.Json.Serialization;
namespace Web.Api.Core.DTO;
public sealed class AccessToken
{
    public string Token { get; }
    public int ExpiresIn { get; }
    [JsonConstructor]
    public AccessToken(string token, int expiresIn) => (Token, ExpiresIn) = (token, expiresIn);
}
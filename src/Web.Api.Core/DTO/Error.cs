using System.Text.Json.Serialization;
namespace Web.Api.Core.DTO;
public sealed class Error
{
    public string Code { get; }
    public string Description { get; }
    [JsonConstructor]
    public Error(string code, string description) => (Code, Description) = (code, description);
}
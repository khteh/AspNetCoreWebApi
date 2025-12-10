using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;

public record LogOutResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public LogOutResponse(bool success, List<Error> errors) : base(success, errors) { }
    [JsonConstructor]
    public LogOutResponse(Guid id, string? username, bool success, List<Error> errors) : base(success, errors) => (Id, UserName) = (id, username);
}
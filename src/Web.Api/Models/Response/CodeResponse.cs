using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;

public record CodeResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public CodeResponse(bool success, List<Error> errors) : base(success, errors) { }
    [JsonConstructor]
    public CodeResponse(Guid id, string? code, bool success, List<Error> errors) : base(success, errors) => (Id, Code) = (id, code);
}
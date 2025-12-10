using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;

public record GenerateNew2FARecoveryCodesResponse : ResponseBase
{
    public Guid Id { get; set; }
    public List<string>? Codes { get; set; }
    public GenerateNew2FARecoveryCodesResponse(bool success, List<Error> errors) : base(success, errors) { }
    [JsonConstructor]
    public GenerateNew2FARecoveryCodesResponse(Guid id, List<string> codes, bool success, List<Error> errors) : base(success, errors) => (Id, Codes) = (id, codes);
}
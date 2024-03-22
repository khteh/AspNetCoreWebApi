using System.Collections.Generic;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record GenerateNew2FARecoveryCodesResponse : ResponseBase
{
    public string Id { get; set; }
    public List<string> Codes { get; set; }
    public GenerateNew2FARecoveryCodesResponse(bool success, List<Core.DTO.Error> errors) : base(success, errors) { }
    public GenerateNew2FARecoveryCodesResponse(string id, List<string> codes, bool success, List<Core.DTO.Error> errors) : base(success, errors) => (Id, Codes) = (id, codes);
}
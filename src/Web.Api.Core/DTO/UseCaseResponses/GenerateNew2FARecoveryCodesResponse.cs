using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseResponses;
public class GenerateNew2FARecoveryCodesResponse : UseCaseResponseMessage
{
    public List<string> Codes { get; init; }
    [JsonConstructor]
    public GenerateNew2FARecoveryCodesResponse(string id, List<string> codes, bool success = false, string message = null, List<Core.DTO.Error> errors = null) : base(string.Empty, success, message, errors) => (Id, Codes) = (id, codes);
}
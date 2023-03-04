using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseResponses;
public class CodeResponse : UseCaseResponseMessage
{
    public string Code { get; init; }
    [JsonConstructor]
    public CodeResponse(string id, string code, bool success = false, string message = null, List<Core.DTO.Error> errors = null) : base(id, success, message, errors) => Code = code;
}
using System.Collections.Generic;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record CodeResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public CodeResponse(bool success, List<Core.DTO.Error> errors) : base(success, errors) { }
    public CodeResponse(Guid id, string code, bool success, List<Core.DTO.Error> errors) : base(success, errors) => (Id, Code) = (id, code);
}
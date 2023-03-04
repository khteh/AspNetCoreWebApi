using System;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;

public class ConfirmEmailRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public Guid IdentityId { get; set; }
    public string Code { get; set; }
    public ConfirmEmailRequest(Guid id, string code) => (IdentityId, Code) = (id, code);
}
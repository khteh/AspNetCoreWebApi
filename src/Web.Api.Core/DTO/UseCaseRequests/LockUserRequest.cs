using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class LockUserRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public string Id { get; }
    public LockUserRequest(string id) => Id = id;
}
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class RefreshSignInRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public string Id { get; set; }
    public RefreshSignInRequest(string id) => Id = id;
}
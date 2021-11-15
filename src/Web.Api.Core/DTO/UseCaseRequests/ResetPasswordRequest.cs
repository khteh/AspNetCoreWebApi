using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class ResetPasswordRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public string Id { get; set; }
    public string NewPassword { get; set; }
    public bool IsFirstLogin { get; set; } = false;
    public ResetPasswordRequest(string id, string newPassword)
    {
        Id = id;
        NewPassword = newPassword;
    }
}
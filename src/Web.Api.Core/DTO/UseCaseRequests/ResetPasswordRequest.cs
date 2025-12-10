using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;

public class ResetPasswordRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Code { get; set; }
    public string NewPassword { get; set; }
    public bool IsFirstLogin { get; set; } = false;
    public ResetPasswordRequest(string? id, string? email, string newPassword, string? code)
    {
        Id = id;
        Email = email;
        NewPassword = newPassword;
        Code = code;
    }
}
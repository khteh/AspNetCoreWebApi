using System;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests
{
    public class ResetPasswordRequest : IUseCaseRequest<UseCaseResponseMessage>
    {
        public bool CreatePasswordChangeRecord { get; set; }
        public string Id { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public bool IsFirstLogin { get; set; } = false;
        public ResetPasswordRequest(string id, string token, string newPassword)
        {
            Id = id;
            Token = token;
            NewPassword = newPassword;
        }
    }
}
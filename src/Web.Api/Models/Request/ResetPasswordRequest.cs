using System;

namespace Web.Api.Models.Request
{
    public class ResetPasswordRequest
    {
        public string Id { get; set; }
        public string NewPassword { get; set; }
        public ResetPasswordRequest() {}
        public ResetPasswordRequest(string id, string password)
        {
            Id = id;
            NewPassword = password;
        }
    }
}
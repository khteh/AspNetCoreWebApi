using System;

namespace Web.Api.Models.Request
{
    public class ChangePasswordRequest
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public ChangePasswordRequest(string id, string oldPassword, string newPassword)
        {
            Id = id;
            Password = oldPassword;
            NewPassword = newPassword;
        }
    }
}
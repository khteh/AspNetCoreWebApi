using System;

namespace Web.Api.Models.Request
{
    public class ChangePasswordRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
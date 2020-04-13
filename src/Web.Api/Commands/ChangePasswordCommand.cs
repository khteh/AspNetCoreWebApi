using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Models.Response;

namespace Web.Api.Commands
{
    public class ChangePasswordCommand : IRequest<ChangePasswordResponse>
    {
        public string IdentityId { get; }
        public string OldPassword { get; }
        public string NewPassword { get; }
        public ChangePasswordCommand(string id, string oldPassword, string newPassword)
        {
            IdentityId = id;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
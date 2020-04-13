using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Models.Response;

namespace Web.Api.Commands
{
    public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
    {
        public string Id { get; }
        public string NewPassword { get; }
        public bool IsFirstLogin { get; } = false;
        public ResetPasswordCommand(string id, string newPassword)
        {
            Id = id;
            NewPassword = newPassword;
        }
    }
}
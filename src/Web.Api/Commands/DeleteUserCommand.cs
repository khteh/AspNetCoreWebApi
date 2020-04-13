using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Models.Response;

namespace Web.Api.Commands
{
    public class DeleteUserCommand : IRequest<DeleteUserResponse>
    {
        public string UserName { get; }
        public DeleteUserCommand(string username) => UserName = username;
    }
}

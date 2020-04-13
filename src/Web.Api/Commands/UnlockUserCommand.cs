using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Models.Response;

namespace Web.Api.Commands
{
    public class UnlockUserCommand : IRequest<LockUserResponse>
    {
        public string Id { get; }
        public UnlockUserCommand(string id) => Id = id;
    }
}
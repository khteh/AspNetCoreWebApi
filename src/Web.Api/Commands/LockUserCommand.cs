using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Models.Response;

namespace Web.Api.Commands
{
    public class LockUserCommand : IRequest<LockUserResponse>
    {
        public string Id { get; }
        public LockUserCommand(string id) => Id = id;
    }
}
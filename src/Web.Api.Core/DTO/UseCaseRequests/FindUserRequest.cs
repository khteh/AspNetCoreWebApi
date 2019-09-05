using System;
using System.Collections.Generic;
using System.Text;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests
{
    public class FindUserRequest : IUseCaseRequest<FindUserResponse>
    {
        public string Email { get; }
        public string UserName { get; }
        public string Id { get; }

        public FindUserRequest(string email, string userName, string id)
        {
            Email = email;
            UserName = userName;
            Id = id;
        }
    }
}
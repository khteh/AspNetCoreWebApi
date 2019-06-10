using System;
using System.Collections.Generic;
using System.Text;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;

namespace Web.Api.Core.Interfaces.UseCases
{
    public interface IFindUserUseCase : IUseCaseRequestHandler<FindUserRequest, FindUserResponse>
    {
    }
}
using System;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;

namespace Web.Api.Core.Interfaces.UseCases
{
    public interface IChangePasswordUseCase : IUseCaseRequestHandler<ChangePasswordRequest, ChangePasswordResponse>
    {
    }
}

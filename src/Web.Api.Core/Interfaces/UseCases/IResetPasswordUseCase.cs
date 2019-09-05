using System;
using Web.Api.Core.DTO.UseCaseRequests;

namespace Web.Api.Core.Interfaces.UseCases
{
    public interface IResetPasswordUseCase : IUseCaseRequestHandler<ResetPasswordRequest, UseCaseResponseMessage>
    {
    }
}
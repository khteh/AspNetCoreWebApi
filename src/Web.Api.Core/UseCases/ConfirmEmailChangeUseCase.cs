using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Helpers;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;

public class ConfirmEmailChangeUseCase : IConfirmEmailChangeUseCase
{
    private readonly IUserRepository _userRepository;
    public ConfirmEmailChangeUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(ConfirmEmailChangeRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        if (!string.IsNullOrEmpty(message.Email) && EmailValidation.IsValidEmail(message.Email) && !string.IsNullOrEmpty(message.Code))
        {
            DTO.GatewayResponses.Repositories.FindUserResponse response = await _userRepository.ConfirmEmailChange(message.IdentityId, message.Email, message.Code);
            await outputPort.Handle(response.Success ? new UseCaseResponseMessage(response.Id, true) : new UseCaseResponseMessage(response.Errors));
            return response.Success;
        }
        else
        {
            await outputPort.Handle(new UseCaseResponseMessage(string.Empty, false, $"Invalid Email {message.Email} / Code {message.Code}!", new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Invalid Email {message.Email} / Code {message.Code}!") }));
            return false;
        }
    }
}
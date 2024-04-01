using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Helpers;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public sealed class ForgotPasswordUseCase : IForgotPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    public ForgotPasswordUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(ForgotPasswordRequest message, IOutputPort<CodeResponse> outputPort)
    {
        if (!string.IsNullOrEmpty(message.Email) && EmailValidation.IsValidEmail(message.Email))
        {
            DTO.GatewayResponses.Repositories.CodeResponse response = await _userRepository.ForgotPassword(message.Email);
            string errMsg = response.Errors != null && response.Errors.Any() ? response.Errors.First().Description : string.Empty;
            await outputPort.Handle(new CodeResponse(response.Id, response.Code, response.Success, errMsg, response.Errors));
            return response.Success;
        }
        else
        {
            await outputPort.Handle(new CodeResponse(string.Empty, string.Empty, false, $"Invalid email {message.Email}!", new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Invalid email {message.Email}!") }));
            return false;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public sealed class FindUserUseCase : IFindUserUseCase
{
    private readonly IUserRepository _userRepository;
    public FindUserUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(FindUserRequest message, IOutputPort<FindUserResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.FindUserResponse response = null;
        if (!string.IsNullOrEmpty(message.Id))
            response = await _userRepository.FindById(message.Id);
        else if (!string.IsNullOrEmpty(message.UserName))
            response = await _userRepository.FindByName(message.UserName);
        else if (!string.IsNullOrEmpty(message.Email))
            response = await _userRepository.FindByEmail(message.Email);
        if (response == null)
        {
            outputPort.Handle(new FindUserResponse(null, null, false, "Invalid request input!", new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid request input!")}));
            return false;
        } else
            outputPort.Handle(response.Success ? new FindUserResponse(response.User, response.Id, true) :
                                                new FindUserResponse(null, null, false, response.Errors.First().Description, response.Errors));
        return response.Success;
    }
    public async Task<bool> FindByEmail(string normalizedEmail, IOutputPort<FindUserResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.FindUserResponse response = await _userRepository.FindByEmail(normalizedEmail);
        if (response == null)
        {
            outputPort.Handle(new FindUserResponse(null, null, false, "Invalid request input!", new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid request input!")}));
            return false;
        } else
            outputPort.Handle(response.Success ? new FindUserResponse(response.User, response.Id, true) :
                                                new FindUserResponse(null, null, false, response.Errors.First().Description, response.Errors));
        return response.Success;
    }
    public async Task<bool> FindById(string userId, IOutputPort<FindUserResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.FindUserResponse response = await _userRepository.FindById(userId);
        if (response == null)
        {
            outputPort.Handle(new FindUserResponse(null, null, false, response.Errors.First().Description, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid request input!")}));
            return false;
        } else
            outputPort.Handle(response.Success ? new FindUserResponse(response.User, response.Id, true) :
                                                new FindUserResponse(null, null, false, response.Errors.First().Description, response.Errors));
        return response.Success;
    }
    public async Task<bool> FindByName(string userName, IOutputPort<FindUserResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.FindUserResponse response = await _userRepository.FindByName(userName);
        if (response == null)
        {
            outputPort.Handle(new FindUserResponse(null, null, false, response.Errors.First().Description, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid request input!")}));
            return false;
        } else
            outputPort.Handle(response.Success ? new FindUserResponse(response.User, response.Id, true) :
                                                new FindUserResponse(null, null, false, response.Errors.First().Description, response.Errors));
        return response.Success;
    }
}
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases;

public class RefreshSignInUseCase : IRefreshSignInUseCase
{
    private readonly IUserRepository _userRepository;
    public RefreshSignInUseCase(IUserRepository repo) => _userRepository = repo;
    public async Task<bool> Handle(RefreshSignInRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        DTO.GatewayResponses.Repositories.FindUserResponse result = await _userRepository.RefreshSignIn(message.Id);
        if (result != null && result.Success && !string.IsNullOrEmpty(result.Id))
        {
            await outputPort.Handle(new UseCaseResponseMessage(result.Id, true, "Signed in successfully!"));
            return true;
        }
        if (result != null)
        {
            string errMsg = result.Errors != null && result.Errors.Any() ? result.Errors.First().Description : string.Empty;
            await outputPort.Handle(new UseCaseResponseMessage(result.Id, result.Success, errMsg, result.Errors));
        }
        else
            await outputPort.Handle(new UseCaseResponseMessage(string.Empty, false, "Invalid username or password.", new List<DTO.Error>() { new DTO.Error(HttpStatusCode.InternalServerError.ToString(), "Failed to refresh sign in!") }));
        return false;
    }
}
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly IUserRepository _userRepository;
    public ChangePasswordUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(ChangePasswordRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        if (string.IsNullOrEmpty(message.IdentityId) || string.IsNullOrEmpty(message.OldPassword) || string.IsNullOrEmpty(message.NewPassword))
        {
            await outputPort.Handle(new UseCaseResponseMessage(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Invalid id {message.IdentityId} / old password {message.OldPassword} / new password {message.NewPassword}") }));
            return false;
        }
        var response = await _userRepository.ChangePassword(message.IdentityId, message.OldPassword, message.NewPassword);
        await outputPort.Handle(response.Success ? new UseCaseResponseMessage(response.Id, true) : new UseCaseResponseMessage(response.Errors!));
        return response.Success;
    }
}
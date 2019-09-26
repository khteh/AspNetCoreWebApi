using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Presenters.Grpc;
using Web.Api.Core.Accounts;
namespace Web.Api.Services
{
    public class AccountsService : Accounts.AccountsBase
    {
        private readonly ILogger<AccountsService> _logger;
        private readonly IRegisterUserUseCase _registerUserUseCase;
        private readonly RegisterUserPresenter _registerUserPresenter;
        private readonly IFindUserUseCase _findUserUseCase;
        private readonly FindUserPresenter _findUserPresenter;
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly DeleteUserPresenter _deleteUserPresenter;
        private readonly IChangePasswordUseCase _changePasswordUseCase;
        private readonly IResetPasswordUseCase _resetPasswordUseCase;
        private readonly ChangePasswordPresenter _changePasswordPresenter;
        private readonly ResetPasswordPresenter _resetPasswordPresenter;
        private readonly ILockUserUseCase _lockUserUseCase;
        private readonly LockUserPresenter _lockUserPresenter;
        public AccountsService(ILogger<AccountsService> logger, IRegisterUserUseCase registerUserUseCase, RegisterUserPresenter registerUserPresenter, IDeleteUserUseCase deleteUserUseCase, DeleteUserPresenter deleteUserPresenter, IFindUserUseCase findUserUseCase, FindUserPresenter findUserPresenter, IChangePasswordUseCase changePasswordUseCase, ChangePasswordPresenter changePasswordPresenter, IResetPasswordUseCase resetPasswordUseCase, ResetPasswordPresenter resetPasswordPresenter)
        {
            _logger = logger;
            _registerUserUseCase = registerUserUseCase;
            _registerUserPresenter = registerUserPresenter;
            _deleteUserUseCase = deleteUserUseCase;
            _deleteUserPresenter = deleteUserPresenter;
            _findUserUseCase = findUserUseCase;
            _findUserPresenter = findUserPresenter;
            _changePasswordUseCase = changePasswordUseCase;
            _changePasswordPresenter = changePasswordPresenter;
            _resetPasswordUseCase = resetPasswordUseCase;
            _resetPasswordPresenter = resetPasswordPresenter;
        }
        public async Task<Web.Api.Core.Grpc.Response> Register(Web.Api.Core.Accounts.RegisterUserRequest request)
        {
            await _registerUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.RegisterUserRequest(request.FirstName, request.LastName, request.Email, request.UserName, request.Password), _registerUserPresenter);
            return _registerUserPresenter.Response;
        }
        // POST api/accounts
        public async Task<Web.Api.Core.Grpc.Response> ChangePassword(Web.Api.Core.Accounts.ChangePasswordRequest request)
        {
            await _changePasswordUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.ChangePasswordRequest(request.Id, request.Password, request.NewPassword), _changePasswordPresenter);
            return _changePasswordPresenter.Response;
        }
        public async Task<Web.Api.Core.Grpc.Response> ResetPassword(Web.Api.Core.Accounts.ResetPasswordRequest request)
        {
            await _resetPasswordUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.ResetPasswordRequest(request.Id, request.NewPassword), _resetPasswordPresenter);
            return _resetPasswordPresenter.Response;
        }

        // POST api/accounts
        public async Task<Web.Api.Core.Accounts.DeleteUserResponse> Delete(StringInputParameter id)
        {
            await _deleteUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.DeleteUserRequest(id.Value), _deleteUserPresenter);
            return _deleteUserPresenter.Response;
        }
        // POST api/accounts/FindById
        public async Task<Web.Api.Core.Accounts.FindUserResponse> FindById(StringInputParameter id)
        {
            await _findUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.FindUserRequest(string.Empty, string.Empty, id.Value), _findUserPresenter);
            return _findUserPresenter.Response;
        }
        // POST api/accounts/FindByUserName
        public async Task<Web.Api.Core.Accounts.FindUserResponse> FindByUserName(StringInputParameter username)
        {
            await _findUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.FindUserRequest(string.Empty, username.Value, string.Empty), _findUserPresenter);
            return _findUserPresenter.Response;
        }
        // POST api/accounts/FindByEmail
        public async Task<Web.Api.Core.Accounts.FindUserResponse> FindByEmail(StringInputParameter email)
        {
            await _findUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.FindUserRequest(email.Value, string.Empty, string.Empty), _findUserPresenter);
            return _findUserPresenter.Response;
        }
        public async Task<Web.Api.Core.Grpc.Response> Lock(StringInputParameter id)
        {
            //=> _service.Lock(id);
            await _lockUserUseCase.Lock(id.Value, _lockUserPresenter);
            return _lockUserPresenter.Response;
        }
        public async Task<Web.Api.Core.Grpc.Response> Unlock(StringInputParameter id)
        {
            //_service.Unlock(id);
            await _lockUserUseCase.UnLock(id.Value, _lockUserPresenter);
            return _lockUserPresenter.Response;
        }
    }
}
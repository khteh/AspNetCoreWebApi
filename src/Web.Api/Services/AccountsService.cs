using Grpc.Core;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Identity.Accounts;
using Web.Api.Presenters.Grpc;
namespace Web.Api.Services;
public class AccountsService : Accounts.AccountsBase
{
    private readonly ILogger<AccountsService> _logger;
    private readonly IRegisterUserUseCase _registerUserUseCase;
    private readonly UserPresenter _userPresenter;
    private readonly IFindUserUseCase _findUserUseCase;
    private readonly FindUserPresenter _findUserPresenter;
    private readonly IDeleteUserUseCase _deleteUserUseCase;
    private readonly UserPresenter _deleteUserPresenter;
    private readonly IChangePasswordUseCase _changePasswordUseCase;
    private readonly IResetPasswordUseCase _resetPasswordUseCase;
    private readonly ChangePasswordPresenter _changePasswordPresenter;
    private readonly ResetPasswordPresenter _resetPasswordPresenter;
    private readonly ILockUserUseCase _lockUserUseCase;
    private readonly LockUserPresenter _lockUserPresenter;
    public AccountsService(ILogger<AccountsService> logger, IRegisterUserUseCase registerUserUseCase, UserPresenter userPresenter,
            IDeleteUserUseCase deleteUserUseCase, UserPresenter deleteUserPresenter,
            IFindUserUseCase findUserUseCase, FindUserPresenter findUserPresenter,
            IChangePasswordUseCase changePasswordUseCase, ChangePasswordPresenter changePasswordPresenter,
            IResetPasswordUseCase resetPasswordUseCase, ResetPasswordPresenter resetPasswordPresenter,
            ILockUserUseCase lockUserUseCase, LockUserPresenter lockUserPresenter)
    {
        _logger = logger;
        _registerUserUseCase = registerUserUseCase;
        _userPresenter = userPresenter;
        _deleteUserUseCase = deleteUserUseCase;
        _deleteUserPresenter = deleteUserPresenter;
        _findUserUseCase = findUserUseCase;
        _findUserPresenter = findUserPresenter;
        _changePasswordUseCase = changePasswordUseCase;
        _changePasswordPresenter = changePasswordPresenter;
        _resetPasswordUseCase = resetPasswordUseCase;
        _resetPasswordPresenter = resetPasswordPresenter;
        _lockUserUseCase = lockUserUseCase;
        _lockUserPresenter = lockUserPresenter;
    }
    public async override Task<Web.Api.Identity.Accounts.UserResponse> Register(Web.Api.Identity.Accounts.RegisterUserRequest request, ServerCallContext context)
    {
        await _registerUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.RegisterUserRequest(request.FirstName, request.LastName, request.Email, request.UserName, request.Password), _userPresenter);
        return _userPresenter.Response;
    }
    // POST api/accounts
    public async override Task<Web.Api.Identity.Response> ChangePassword(Web.Api.Identity.Accounts.ChangePasswordRequest request, ServerCallContext context)
    {
        await _changePasswordUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.ChangePasswordRequest(request.Id, request.Password, request.NewPassword), _changePasswordPresenter);
        return _changePasswordPresenter.Response;
    }
    public async override Task<Web.Api.Identity.Response> ResetPassword(Web.Api.Identity.Accounts.ResetPasswordRequest request, ServerCallContext context)
    {
        await _resetPasswordUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.ResetPasswordRequest(request.Id, request.Email, request.NewPassword, request.Code), _resetPasswordPresenter);
        return _resetPasswordPresenter.Response;
    }
    // POST api/accounts
    public async override Task<Web.Api.Identity.Accounts.UserResponse> Delete(StringInputParameter id, ServerCallContext context)
    {
        await _deleteUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.DeleteUserRequest(id.Value), _deleteUserPresenter);
        return _deleteUserPresenter.Response;
    }
    // POST api/accounts/FindById
    public async override Task<Web.Api.Identity.Accounts.FindUserResponse> FindById(StringInputParameter id, ServerCallContext context)
    {
        await _findUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.FindUserRequest(string.Empty, string.Empty, id.Value), _findUserPresenter);
        return _findUserPresenter.Response;
    }
    // POST api/accounts/FindByUserName
    public async override Task<Web.Api.Identity.Accounts.FindUserResponse> FindByUserName(StringInputParameter username, ServerCallContext context)
    {
        await _findUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.FindUserRequest(string.Empty, username.Value, string.Empty), _findUserPresenter);
        return _findUserPresenter.Response;
    }
    // POST api/accounts/FindByEmail
    public async override Task<Web.Api.Identity.Accounts.FindUserResponse> FindByEmail(StringInputParameter email, ServerCallContext context)
    {
        await _findUserUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.FindUserRequest(email.Value, string.Empty, string.Empty), _findUserPresenter);
        return _findUserPresenter.Response;
    }
    public async override Task<Web.Api.Identity.Response> Lock(StringInputParameter id, ServerCallContext context)
    {
        await _lockUserUseCase.Lock(id.Value, _lockUserPresenter);
        return _lockUserPresenter.Response;
    }
    public async override Task<Web.Api.Identity.Response> UnLock(StringInputParameter id, ServerCallContext context)
    {
        await _lockUserUseCase.UnLock(id.Value, _lockUserPresenter);
        return _lockUserPresenter.Response;
    }
}
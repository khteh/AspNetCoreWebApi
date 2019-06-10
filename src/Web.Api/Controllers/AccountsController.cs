using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Presenters;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IRegisterUserUseCase _registerUserUseCase;
        private readonly RegisterUserPresenter _registerUserPresenter;
        private readonly IFindUserUseCase _findUserUseCase;
        private readonly FindUserPresenter _findUserPresenter;
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly DeleteUserPresenter _deleteUserPresenter;
        private readonly IChangePasswordUseCase _changePasswordUseCase;
        private readonly ChangePasswordPresenter _changePasswordPresenter;

        public AccountsController(IRegisterUserUseCase registerUserUseCase, RegisterUserPresenter registerUserPresenter, IDeleteUserUseCase deleteUserUseCase, DeleteUserPresenter deleteUserPresenter, IFindUserUseCase findUserUseCase, FindUserPresenter findUserPresenter, IChangePasswordUseCase changePasswordUseCase, ChangePasswordPresenter changePasswordPresenter)
        {
            _registerUserUseCase = registerUserUseCase;
            _registerUserPresenter = registerUserPresenter;
            _deleteUserUseCase = deleteUserUseCase;
            _deleteUserPresenter = deleteUserPresenter;
            _findUserUseCase = findUserUseCase;
            _findUserPresenter = findUserPresenter;
            _changePasswordUseCase = changePasswordUseCase;
            _changePasswordPresenter = changePasswordPresenter;
        }

        // POST api/accounts/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] Models.Request.RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _registerUserUseCase.Handle(new RegisterUserRequest(request.FirstName, request.LastName, request.Email, request.UserName, request.Password), _registerUserPresenter);
            return _registerUserPresenter.ContentResult;
        }
        // POST api/accounts
        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword([FromBody] Models.Request.ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _changePasswordUseCase.Handle(new ChangePasswordRequest(request.Id, request.Password, request.NewPassword), _changePasswordPresenter);
            return _changePasswordPresenter.ContentResult;
        }

        // POST api/accounts
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _deleteUserUseCase.Handle(new DeleteUserRequest(id), _deleteUserPresenter);
            return _deleteUserPresenter.ContentResult;
        }
        // POST api/accounts/FindById
        [HttpGet("id/{id}")]
        public async Task<ActionResult> FindById(string id)
        {
            await _findUserUseCase.Handle(new FindUserRequest(string.Empty, string.Empty, id), _findUserPresenter);
            return _findUserPresenter.ContentResult;
        }
        // POST api/accounts/FindByUserName
        [HttpGet("username/{username}")]
        public async Task<ActionResult> FindByUserName(string username)
        {
            await _findUserUseCase.Handle(new FindUserRequest(string.Empty, username, string.Empty), _findUserPresenter);
            return _findUserPresenter.ContentResult;
        }
        // POST api/accounts/FindByEmail
        [HttpGet("email/{email}")]
        public async Task<ActionResult> FindByEmail(string email)
        {
            await _findUserUseCase.Handle(new FindUserRequest(email, string.Empty, string.Empty), _findUserPresenter);
            return _findUserPresenter.ContentResult;
        }
    }
}
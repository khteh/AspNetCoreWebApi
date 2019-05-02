using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Core.Dto.UseCaseRequests;
using Web.Api.Core.Dto.UseCaseResponses;
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
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly DeleteUserPresenter _deleteUserPresenter;

        public AccountsController(IRegisterUserUseCase registerUserUseCase, RegisterUserPresenter registerUserPresenter, IDeleteUserUseCase deleteUserUseCase, DeleteUserPresenter deleteUserPresenter)
        {
            _registerUserUseCase = registerUserUseCase;
            _registerUserPresenter = registerUserPresenter;
            _deleteUserUseCase = deleteUserUseCase;
            _deleteUserPresenter = deleteUserPresenter;
        }

        // POST api/accounts
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Models.Request.RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _registerUserUseCase.Handle(new RegisterUserRequest(request.FirstName, request.LastName, request.Email, request.UserName, request.Password), _registerUserPresenter);
            return _registerUserPresenter.ContentResult;
        }

        // POST api/accounts
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _deleteUserUseCase.Handle(new DeleteUserRequest(id), _deleteUserPresenter);
            return _deleteUserPresenter.ContentResult;
        }
    }
}
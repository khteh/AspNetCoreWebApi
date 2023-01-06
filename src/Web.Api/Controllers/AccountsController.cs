using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Api.Commands;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IFindUserUseCase _findUserUseCase;
    private readonly FindUserPresenter _findUserPresenter;
    public AccountsController(IMediator mediator, IMapper mapper,
            IFindUserUseCase findUserUseCase, FindUserPresenter findUserPresenter)
    {
        _mediator = mediator;
        _mapper = mapper;
        _findUserUseCase = findUserUseCase;
        _findUserPresenter = findUserPresenter;
    }

    // POST api/accounts/register
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] Models.Request.RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        RegisterUserResponse response = await _mediator.Send(new RegisterUserCommand(request.FirstName, request.LastName, request.Email, request.UserName, request.Password));
        return _mapper.Map<JsonContentResult>(response);
    }
    // POST api/accounts
    [HttpPost("changepassword")]
    public async Task<ActionResult> ChangePassword([FromBody] Models.Request.ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        ChangePasswordResponse response = await _mediator.Send(new ChangePasswordCommand(request.Id, request.Password, request.NewPassword));
        return _mapper.Map<JsonContentResult>(response);
    }
    [HttpPost("resetpassword")]
    public async Task<ActionResult> ResetPassword([FromBody] Models.Request.ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        ResetPasswordResponse response = await _mediator.Send(new ResetPasswordCommand(request.Id, request.Email, request.NewPassword, request.Code));
        return _mapper.Map<JsonContentResult>(response);
    }
    // POST api/accounts
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        DeleteUserResponse response = await _mediator.Send(new DeleteUserCommand(id));
        return _mapper.Map<JsonContentResult>(response);
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
    [HttpGet("lock/{id}")]
    public async Task<ActionResult> Lock(string id)
    {
        //=> _service.Lock(id);
        LockUserResponse response = await _mediator.Send(new LockUserCommand(id));
        return _mapper.Map<JsonContentResult>(response);
    }
    [HttpGet("unlock/{id}")]
    public async Task<ActionResult> Unlock(string id)
    {
        //_service.Unlock(id);
        LockUserResponse response = await _mediator.Send(new UnlockUserCommand(id));
        return _mapper.Map<JsonContentResult>(response);
    }
}
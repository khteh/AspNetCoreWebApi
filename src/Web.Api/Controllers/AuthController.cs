using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Api.Commands;
using Web.Api.Infrastructure.Auth;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly AuthSettings _authSettings;
    public AuthController(IMediator mediator, IOptions<AuthSettings> authSettings, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
        _authSettings = authSettings.Value;
    }
    // POST api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> Login([FromBody] Models.Request.LogInRequest request)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }
        LogInResponse response = await _mediator.Send(new LogInCommand(request.UserName, request.Password, Request.HttpContext.Connection.RemoteIpAddress?.ToString()));
        return _mapper.Map<JsonContentResult>(response);
    }
    // POST api/auth/refreshtoken
    [HttpPost("refreshtoken")]
    public async Task<ActionResult> RefreshToken([FromBody] Models.Request.ExchangeRefreshTokenRequest request)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }
        ExchangeRefreshTokenResponse response = await _mediator.Send(new ExchangeRefreshTokenCommand(request.AccessToken, request.RefreshToken, _authSettings.SecretKey));
        return _mapper.Map<JsonContentResult>(response);
    }
}
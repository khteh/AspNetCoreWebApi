using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Settings;
using Web.Api.Presenters;
using Web.Api.Serialization;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginUseCase _loginUseCase;
        private readonly LoginPresenter _loginPresenter;
        private readonly IExchangeRefreshTokenUseCase _exchangeRefreshTokenUseCase;
        private readonly ExchangeRefreshTokenPresenter _exchangeRefreshTokenPresenter;
        private readonly AuthSettings _authSettings;
        public AuthController(ILoginUseCase loginUseCase, LoginPresenter loginPresenter, IExchangeRefreshTokenUseCase exchangeRefreshTokenUseCase, ExchangeRefreshTokenPresenter exchangeRefreshTokenPresenter, IOptions<AuthSettings> authSettings)
        {
            _loginUseCase = loginUseCase;
            _loginPresenter = loginPresenter;
            _exchangeRefreshTokenUseCase = exchangeRefreshTokenUseCase;
            _exchangeRefreshTokenPresenter = exchangeRefreshTokenPresenter;
            _authSettings = authSettings.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([FromBody] Models.Request.LoginRequest request)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _loginUseCase.Handle(new LoginRequest(request.UserName, request.Password, Request.HttpContext.Connection.RemoteIpAddress?.ToString(), request.RememberMe, true, false), _loginPresenter);
            Models.Response.LoginResponse response = JsonSerializer.DeSerializeObject<Models.Response.LoginResponse>(_loginPresenter.ContentResult.Content);
            //Response.Headers.Add("Authorization", $"Bearer {response.AccessToken.Token}");  Doesn't work
            //Response.Headers.Add("www-authenticate", $"Bearer {response.AccessToken.Token}"); Doesn't work
            //return LocalRedirect($"/api/protected?access_token={response.AccessToken.Token}"); This works
            return _loginPresenter.ContentResult;
        }

        // POST api/auth/refreshtoken
        [HttpPost("refreshtoken")]
        public async Task<ActionResult> RefreshToken([FromBody] Models.Request.ExchangeRefreshTokenRequest request)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState);}
            await _exchangeRefreshTokenUseCase.Handle(new ExchangeRefreshTokenRequest(request.AccessToken, request.RefreshToken, _authSettings.SecretKey), _exchangeRefreshTokenPresenter);
            return _exchangeRefreshTokenPresenter.ContentResult;
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Presenters;
using Web.Api.Models.Response;
namespace Web.Api.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IRegisterUserUseCase _useCase;
        private readonly RegisterUserPresenter _presenter;
        public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger, IRegisterUserUseCase useCase, RegisterUserPresenter presenter)
        {
            _logger = logger;
            _useCase = useCase;
            _presenter = presenter;
        }
        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            await _useCase.Handle(new RegisterUserRequest(request.FirstName, request.LastName, request.Email, request.UserName, request.Password), _presenter);
            return _presenter.Response;
        }
    }
}
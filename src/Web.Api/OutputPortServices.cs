using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Presenters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OutputPortServices
    {
        public static IServiceCollection AddOutputPorts(this IServiceCollection service) =>
            service.AddSingleton<IOutputPort<ExchangeRefreshTokenResponse>, ExchangeRefreshTokenPresenter>()
                .AddSingleton<IOutputPort<LoginResponse>, LoginPresenter>()
                .AddSingleton<IOutputPort<RegisterUserResponse>, RegisterUserPresenter>()
                .AddSingleton<IOutputPort<DeleteUserResponse>, DeleteUserPresenter>()
                .AddSingleton<ExchangeRefreshTokenPresenter>()
                .AddSingleton<LoginPresenter>()
                .AddSingleton<RegisterUserPresenter>()
                .AddSingleton<FindUserPresenter>()
                .AddSingleton<DeleteUserPresenter>()
                .AddSingleton<ChangePasswordPresenter>();
    }
}
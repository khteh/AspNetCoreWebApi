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
                .AddSingleton<ChangePasswordPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.ChangePasswordPresenter>()
                .AddSingleton<RegisterUserPresenter>()
                .AddSingleton<DeleteUserPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.DeleteUserPresenter>()
                .AddSingleton<ExchangeRefreshTokenPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.ExchangeRefreshTokenPresenter>()
                .AddSingleton<FindUserPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.FindUserPresenter>()
                .AddSingleton<LockUserPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.LockUserPresenter>()
                .AddSingleton<LoginPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.LoginPresenter>()
                .AddSingleton<RegisterUserPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.RegisterUserPresenter>()
                .AddSingleton<ResetPasswordPresenter>()
                .AddSingleton<Web.Api.Presenters.Grpc.ResetPasswordPresenter>();
    }
}
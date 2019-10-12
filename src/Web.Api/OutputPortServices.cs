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
            service.AddScoped<IOutputPort<ExchangeRefreshTokenResponse>, ExchangeRefreshTokenPresenter>()
                .AddScoped<IOutputPort<LoginResponse>, LoginPresenter>()
                .AddScoped<ChangePasswordPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.ChangePasswordPresenter>()
                .AddScoped<RegisterUserPresenter>()
                .AddScoped<DeleteUserPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.DeleteUserPresenter>()
                .AddScoped<ExchangeRefreshTokenPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.ExchangeRefreshTokenPresenter>()
                .AddScoped<FindUserPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.FindUserPresenter>()
                .AddScoped<LockUserPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.LockUserPresenter>()
                .AddScoped<LoginPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.LoginPresenter>()
                .AddScoped<RegisterUserPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.RegisterUserPresenter>()
                .AddScoped<ResetPasswordPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.ResetPasswordPresenter>();
    }
}
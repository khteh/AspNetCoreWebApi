using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Presenters;
namespace Microsoft.Extensions.DependencyInjection;
public static class OutputPortServices
{
    public static IServiceCollection AddOutputPorts(this IServiceCollection service) =>
        service.AddScoped<CodePresenter>()
                .AddScoped<IOutputPort<ExchangeRefreshTokenResponse>, ExchangeRefreshTokenPresenter>()
                .AddScoped<IOutputPort<LogInResponse>, LogInPresenter>()
                .AddScoped<IOutputPort<SignInResponse>, SignInPresenter>()
                .AddScoped<ChangePasswordPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.ChangePasswordPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.RegistrationConfirmationPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.EmailConfirmationPresenter>()
                .AddScoped<GenerateNew2FARecoveryCodesPresenter>()
                .AddScoped<DeleteUserPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.UserPresenter>()
                .AddScoped<ExchangeRefreshTokenPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.ExchangeRefreshTokenPresenter>()
                .AddScoped<FindUserPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.FindUserPresenter>()
                .AddScoped<GenerateNew2FARecoveryCodesPresenter>()
                .AddScoped<LockUserPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.LockUserPresenter>()
                .AddScoped<LogInPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.LogInPresenter>()
                .AddScoped<SignInPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.SignInPresenter>()
                .AddScoped<RegisterUserPresenter>()
                .AddScoped<RegistrationConfirmationPresenter>()
                .AddScoped<ResetPasswordPresenter>()
                .AddScoped<Web.Api.Presenters.Grpc.ResetPasswordPresenter>()
                .AddScoped<SimplePresenter>()
                .AddScoped<SignInPresenter>();
}
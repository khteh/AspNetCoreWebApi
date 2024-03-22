using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Core.UseCases;
namespace Microsoft.Extensions.DependencyInjection;
public static class CoreServices
{
    public static IServiceCollection AddCore(this IServiceCollection service) =>
            service.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>()
            .AddScoped<IDeleteUserUseCase, DeleteUserUseCase>()
            .AddScoped<IExchangeRefreshTokenUseCase, ExchangeRefreshTokenUseCase>()
            .AddScoped<IFindUserUseCase, FindUserUseCase>()
            .AddScoped<IGenerateNew2FARecoveryCodesUseCase, GenerateNew2FARecoveryCodesUseCase>()
            .AddScoped<ILockUserUseCase, LockUserUseCase>()
            .AddScoped<ILogInUseCase, LogInUseCase>()
            .AddScoped<IRegisterUserUseCase, RegisterUserUseCase>()
            .AddScoped<IRegistrationConfirmationUseCase, RegistrationConfirmationUseCase>()
            .AddScoped<IConfirmEmailUseCase, ConfirmEmailUseCase>()
            .AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>()
            .AddScoped<ISignInUseCase, SignInUseCase>();
}
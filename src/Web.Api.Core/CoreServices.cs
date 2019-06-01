using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Core.UseCases;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddCore(this IServiceCollection service) =>
            service.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>()
            .AddScoped<IFindUserUseCase, FindUserUseCase>()
            .AddScoped<IDeleteUserUseCase, DeleteUserUseCase>()
            .AddScoped<ILoginUseCase, LoginUseCase>()
            .AddScoped<IExchangeRefreshTokenUseCase, ExchangeRefreshTokenUseCase>()
            .AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
    }
}
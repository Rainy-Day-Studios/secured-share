using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Common;
using UseCases.SecretManagement;
using UseCases.SecretManagement.CreateSecret;

namespace UseCases.Injection;

public static class UseCasesInjector
{
    public static IServiceCollection InjectUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICreateSecretUseCase, CreateSecretUseCase>();
        services.AddScoped<IGetSecretUseCase, GetSecretUseCase>();

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        services.AddValidatorsFromAssemblyContaining<CreateSecretValidator>();

        return services;
    }
}
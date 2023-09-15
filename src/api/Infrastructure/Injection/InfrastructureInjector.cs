using Infrastructure.KeyProvider.KeyVault;
using Infrastructure.SecretPersistence.TableStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UseCases.SecretManagement;

namespace Infrastructure.Injection;

public static class InfrastructureInjector
{
    public static IServiceCollection InjectInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var tsConfig = new TableStorageConfig();
        config.Bind("Values:SecretPersistence:AzureTableStorage", tsConfig);

        services.AddScoped(fac => tsConfig);
        services.AddScoped<ISecretStore, TableStorageAccess>();


        var kvConfig = new KeyVaultConfig();
        config.Bind("Values:KeyProvider:KeyVault", kvConfig);

        services.AddScoped(fac => kvConfig);
        services.AddScoped<IKeyProvider, KeyVaultKeyProvider>();

        return services;
    }
}

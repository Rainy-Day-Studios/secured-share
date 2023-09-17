using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using UseCases.SecretManagement;

namespace Infrastructure.KeyProvider.KeyVault;

public class KeyVaultKeyProvider : IKeyProvider
{
    private readonly SecretClient _secretClient;

    public KeyVaultKeyProvider(KeyVaultConfig config)
    {
        var tokenCredential = new DefaultAzureCredential();

        _secretClient = new SecretClient(new Uri(config.VaultUri), tokenCredential);
    }

    public async Task<string> GetSecretEncryptionKey()
    {
        var secret = await _secretClient.GetSecretAsync("secret-encryption-key");
        return secret.Value.Value;
    }
}
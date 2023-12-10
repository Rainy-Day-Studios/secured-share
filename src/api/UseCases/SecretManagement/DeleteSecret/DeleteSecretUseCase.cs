using Entities;
using Microsoft.Extensions.Logging;
using UseCases.Result;

namespace UseCases.SecretManagement.CreateSecret;

public interface IDeleteSecretUseCase
{
    Task<Result<int>> DeleteSecret(string secretId);
}

public class DeleteSecretUseCase : IDeleteSecretUseCase
{
    private readonly ISecretStore _secretStore;
    private readonly IKeyProvider _keyProvider;

    private readonly ILogger _logger;

    public DeleteSecretUseCase(
        ISecretStore secretStore,
        IKeyProvider keyProvider,
        ILoggerFactory loggerFac)
    {
        _secretStore = secretStore;
        _keyProvider = keyProvider;

        _logger = loggerFac.CreateLogger<DeleteSecretUseCase>();
    }

    public async Task<Result<int>> DeleteSecret(string secretId)
    {
        try
        {
            var secret = _secretStore.GetSecret(secretId);
            if (secret == null)
            {
                return Result<int>.Fail(ResultReason.NotFound, "Secret was not found.");
            }

            secret.EncryptedValue = string.Empty;
            secret.InternalIV = string.Empty;
            secret.ClientIV = string.Empty;
            secret.Metadata.Password = string.Empty;
            secret.Metadata.Salt = string.Empty;

            await _secretStore.UpdateSecret(secret);

            return Result<int>.Succeed(0);
        }
        catch (Exception ex)
        {
            const string errMsg = "Error deleting secret.";
            _logger.LogError(ex, errMsg);
            return Result<int>.Fail(errMsg);
        }
    }
}
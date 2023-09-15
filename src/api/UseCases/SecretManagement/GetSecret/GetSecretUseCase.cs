using Entities;
using Microsoft.Extensions.Logging;
using UseCases.Common;
using UseCases.Result;
using UseCases.SecretManagement.DTO;

namespace UseCases.SecretManagement;

public interface IGetSecretUseCase
{
    Result<SecurityMetadata> GetSecretMetadata(string secretId);
    Task<Result<SecuredSecret>> GetSecret(GetSecretRequest secretRequest);
}

public class GetSecretUseCase : IGetSecretUseCase
{
    private readonly ISecretStore _secretStore;
    private readonly IKeyProvider _keyProvider;
    private readonly IDateTimeProvider _dtProvider;

    private readonly ILogger _logger;

    public GetSecretUseCase(
        ISecretStore secretStore,
        IKeyProvider keyProvider,
        IDateTimeProvider dtProvider,
        ILoggerFactory loggerFac)
    {
        _secretStore = secretStore;
        _keyProvider = keyProvider;
        _dtProvider = dtProvider;

        _logger = loggerFac.CreateLogger<GetSecretUseCase>();
    }

    public Result<SecurityMetadata> GetSecretMetadata(string secretId)
    {
        try
        {
            var secret = _secretStore.GetSecret(secretId);

            if (secret == null || secret.Metadata.IsExpired(_dtProvider.UtcNow))
            {
                return Result<SecurityMetadata>.Fail(
                    ResultReason.NotFound,
                    "Secret not found. It either expired or you have an invalid link.");
            }

            return Result<SecurityMetadata>.Succeed(secret.Metadata);
        }
        catch (Exception ex)
        {
            const string errMsg = "Error retrieving secret metadata.";
            _logger.LogError(ex, errMsg);
            return Result<SecurityMetadata>.Fail(errMsg);
        }
    }

    public async Task<Result<SecuredSecret>> GetSecret(GetSecretRequest secretRequest)
    {
        try
        {
            var secret = _secretStore.GetSecret(secretRequest.SecretId);

            if (secret == null || secret.Metadata.IsExpired(_dtProvider.UtcNow))
            {
                return Result<SecuredSecret>.Fail(
                    ResultReason.NotFound,
                    "Secret not found. It either expired or you have an invalid link.");
            }

            if (secret.Metadata.RequiresPassword)
            {
                var hashedProvidedPassword = Hasher.ComputeHash(secretRequest.Password, secret.Metadata.Salt);
                if (hashedProvidedPassword != secret.Metadata.Password)
                {
                    return Result<SecuredSecret>.Fail(
                        ResultReason.InvalidRequest,
                        "Invalid password."); 
                }
            }

            if (secret.Metadata.MaxAccessCount.HasValue)
            {
                var accessHistory = _secretStore.GetAccessHistory(secret.Id);
                secret.AccessHistory = accessHistory;

                if (secret.MaxAccessCountExceeded)
                {
                    return Result<SecuredSecret>.Fail(
                        ResultReason.InvalidRequest,
                        "The maximum number of views for this secret has been exceeded.");
                }
                await _secretStore.CreateAccessEvent(new AccessEvent { EventDate = _dtProvider.UtcNow }, secret);
            }

            var encryptionKey = await _keyProvider.GetSecretEncryptionKey();
            secret.EncryptedValue = Encryptor.Decrypt(encryptionKey, secret.InternalIV, secret.EncryptedValue);

            return Result<SecuredSecret>.Succeed(secret);
        }
        catch (Exception ex)
        {
            const string errMsg = "Error retrieving secret.";
            _logger.LogError(ex, errMsg);
            return Result<SecuredSecret>.Fail(errMsg);
        }
    }
}
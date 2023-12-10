using Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UseCases.Common;
using UseCases.Result;
using UseCases.Validation;

namespace UseCases.SecretManagement.CreateSecret;

public interface ICreateSecretUseCase
{
    Task<Result<SecuredSecret>> CreateSecret(SecuredSecret newSecret);
}

public class CreateSecretUseCase : ICreateSecretUseCase
{
    private readonly ISecretStore _secretStore;
    private readonly IKeyProvider _keyProvider;
    private readonly IValidator<SecuredSecret> _secretValidator;
    private readonly IDateTimeProvider _dtProvider;
    private readonly ILogger _logger;

    public CreateSecretUseCase(
        ISecretStore secretStore,
        IKeyProvider keyProvider,
        IValidator<SecuredSecret> secretValidator,
        IDateTimeProvider dtProvider,
        ILoggerFactory loggerFac)
    {
        _secretStore = secretStore;
        _keyProvider = keyProvider;

        _secretValidator = secretValidator;
        _dtProvider = dtProvider;
        _logger = loggerFac.CreateLogger<CreateSecretUseCase>();
    }

    public async Task<Result<SecuredSecret>> CreateSecret(SecuredSecret newSecret)
    {
        try
        {
            var validationResult = _secretValidator.Validate(newSecret);
            if (!validationResult.IsValid)
            {
                return Result<SecuredSecret>.Fail(
                    ResultReason.ValidationFailed,
                    validationResult.FormatValidationErrorMessage());
            }

            var encryptionKey = await _keyProvider.GetSecretEncryptionKey();
            var encryptionResult = Encryptor.Encrypt(encryptionKey, newSecret.EncryptedValue);

            newSecret.EncryptedValue = encryptionResult.Value;
            newSecret.InternalIV = encryptionResult.IV;
            
            newSecret.CreatedDate = _dtProvider.UtcNow;

            if (newSecret.Metadata.RequiresPassword)
            {
                newSecret.Metadata.Salt = Guid.NewGuid().ToString();
                newSecret.Metadata.Password = Hasher.ComputeHash(newSecret.Metadata.Password, newSecret.Metadata.Salt);
            }

            var securedSecret = await _secretStore.CreateSecret(newSecret);

            return Result<SecuredSecret>.Succeed(securedSecret);
        }
        catch (Exception ex)
        {
            const string errMsg = "Error creating secret.";
            _logger.LogError(ex, errMsg);
            return Result<SecuredSecret>.Fail(errMsg);
        }
    }
}
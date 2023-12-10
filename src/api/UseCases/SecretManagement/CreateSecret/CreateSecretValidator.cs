using Entities;
using FluentValidation;
using UseCases.Common;

namespace UseCases.SecretManagement;

public class CreateSecretValidator : AbstractValidator<SecuredSecret>
{
    public CreateSecretValidator(IDateTimeProvider dtProvider)
    {
        RuleFor(secret => secret.EncryptedValue)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(secret => secret.ClientIV)
            .NotEmpty()
            .WithName(nameof(SecuredSecret.ClientIV));

        RuleFor(secret => secret.Metadata.Expiration)
            .GreaterThan(dtProvider.UtcNow)
            .WithName(nameof(SecuredSecret.Metadata.Expiration))
            .WithMessage("Expiration must be in the future.");

        RuleFor(secret => secret.Metadata.MaxAccessCount)
            .GreaterThan(0)
            .WithName(nameof(SecuredSecret.Metadata.MaxAccessCount))
            .WithMessage("Max access count must be greater than 0.");
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using UseCases.Result;
using UseCases.SecretManagement;
using UseCases.SecretManagement.CreateSecret;
using Xunit;

namespace Tests.UseCases.UnitTests.SecretManagement;

public class CreateSecretUseCaseTests
{
    private readonly ISecretStore _secretStore = A.Fake<ISecretStore>(opts => opts.Strict());
    private readonly IKeyProvider _keyProvider = A.Fake<IKeyProvider>(opts => opts.Strict());
    private readonly IValidator<SecuredSecret> _validator = A.Fake<IValidator<SecuredSecret>>();
    private readonly ILoggerFactory _loggerFac = A.Fake<ILoggerFactory>();

    private readonly CreateSecretUseCase _useCase;

    public CreateSecretUseCaseTests()
    {
        _useCase = new CreateSecretUseCase(_secretStore, _keyProvider, _validator, _loggerFac);
    }

    [Fact]
    public async Task CreateSecret_ValidSecretRequest_ReturnsSuccessAndCreatesSecret()
    {
        // Arrange 
        var req = new SecuredSecret
        {
            EncryptedValue = "abc",
            Metadata = new SecurityMetadata { Expiration = new DateTime(2022, 05, 21) }
        };

        A.CallTo(() => _validator.Validate(req)).Returns(new ValidationResult());
        A.CallTo(() => _secretStore.CreateSecret(req)).Returns(new SecuredSecret());
        A.CallTo(() => _keyProvider.GetSecretEncryptionKey()).Returns("The_SecretKey_12");

        // Act
        var result = await _useCase.CreateSecret(req);

        // Assert
        result.Success.Should().BeTrue();
        A.CallTo(() => _secretStore.CreateSecret(req)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CreateSecret_InvalidSecretRequest_ReturnsFailure()
    {
        // Arrange 
        var req = new SecuredSecret
        {
            EncryptedValue = "abc",
            Metadata = new SecurityMetadata { Expiration = new DateTime(2022, 05, 21) }

        };

        A.CallTo(() => _validator.Validate(req)).Returns(new ValidationResult(new List<ValidationFailure>
        {
            new() { PropertyName = "prop", ErrorMessage = "err" }
        }));

        // Act
        var result = await _useCase.CreateSecret(req);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.ValidationFailed);
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Entities;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using UseCases;
using UseCases.Common;
using UseCases.Result;
using UseCases.SecretManagement;
using UseCases.SecretManagement.DTO;
using Xunit;

namespace Tests.UnitTests.UseCases.SecretManagement;

public class GetSecretUseCaseTests
{
    private readonly ISecretStore _secretStore = A.Fake<ISecretStore>(opts => opts.Strict());
    private readonly IKeyProvider _keyProvider = A.Fake<IKeyProvider>(opts => opts.Strict());
    private readonly IDateTimeProvider _dtProvider = A.Fake<IDateTimeProvider>();
    private readonly DateTime _utcNow = new(2022, 05, 21);
    private readonly ILoggerFactory _loggerFac = A.Fake<ILoggerFactory>();

    private readonly GetSecretUseCase _useCase;

    public GetSecretUseCaseTests()
    {
        A.CallTo(() => _dtProvider.UtcNow).Returns(_utcNow);
        A.CallTo(() => _keyProvider.GetSecretEncryptionKey()).Returns("The_SecretKey_12");

        _useCase = new GetSecretUseCase(_secretStore, _keyProvider, _dtProvider, _loggerFac);
    }

    [Fact]
    public void GetSecretMetadata_SecretFoundWithFutureExpiration_ReturnsMetadata()
    {
        // Arrange 
        const string secretId = "secret-123";

        var foundModel = new SecuredSecret
        {
            Id = secretId,
            EncryptedValue = "abc",
            Metadata = new SecurityMetadata { Expiration = _utcNow.AddDays(7) }
        };

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(foundModel);

        // Act
        var result = _useCase.GetSecretMetadata(secretId);

        // Assert
        result.Success.Should().BeTrue();
        result.Model.Should().Be(foundModel.Metadata);
    }

    [Fact]
    public void GetSecretMetadata_SecretNotFound_ReturnsNotFound()
    {
        // Arrange 
        const string secretId = "secret-123";

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(null);

        // Act
        var result = _useCase.GetSecretMetadata(secretId);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.NotFound);
    }

    [Fact]
    public void GetSecretMetadata_SecretFoundButExpired_ReturnsNotFound()
    {
        // Arrange 
        const string secretId = "secret-123";

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(new SecuredSecret
        {
            Metadata = new SecurityMetadata { Expiration = _utcNow.AddMinutes(-1) }
        });

        // Act
        var result = _useCase.GetSecretMetadata(secretId);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.NotFound);
    }

    [Fact]
    public async Task GetSecret_SecretNotFound_ReturnsNotFound()
    {
        // Arrange 
        const string secretId = "secret-123";

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(null);

        // Act
        var result = await _useCase.GetSecret(new GetSecretRequest
        {
            SecretId = secretId,
            Password = "Password1!"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.NotFound);
    }

    [Fact]
    public async Task GetSecret_SecretExpired_ReturnsNotFound()
    {
        // Arrange 
        const string secretId = "secret-123";
        var foundSecret = GetValidSecret();
        foundSecret.Metadata = new SecurityMetadata { Expiration = _utcNow.AddSeconds(-1) };

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(foundSecret);

        // Act
        var result = await _useCase.GetSecret(new GetSecretRequest
        {
            SecretId = secretId,
            Password = "Password1!"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.NotFound);
    }

    [Fact]
    public async Task GetSecret_SecretCleared_ReturnsNotFound()
    {
        // Arrange 
        const string secretId = "secret-123";
        var foundSecret = GetValidSecret();
        foundSecret.EncryptedValue = "";

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(foundSecret);

        // Act
        var result = await _useCase.GetSecret(new GetSecretRequest
        {
            SecretId = secretId,
            Password = "Password1!"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.NotFound);
    }

    [Fact]
    public async Task GetSecret_PasswordRequiredWithInvalidPassword_ReturnsInvalidRequest()
    {
        // Arrange 
        const string secretId = "secret-123";

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(GetValidSecret());

        // Act
        var result = await _useCase.GetSecret(new GetSecretRequest
        {
            SecretId = secretId,
            Password = "Password2!"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.InvalidRequest);
    }

    [Fact]
    public async Task GetSecret_PasswordRequiredWithValidPassword_ReturnsSecret()
    {
        // Arrange 
        const string secretId = "secret-123";

        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(GetValidSecret());

        // Act
        var result = await _useCase.GetSecret(new GetSecretRequest
        {
            SecretId = secretId,
            Password = "Password1!"
        });

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetSecret_MaxAccessCountNotExceeded_ReturnsSecret()
    {
        // Arrange 
        const string secretId = "secret-123";

        var validSecret = GetValidSecret();
        validSecret.Metadata.MaxAccessCount = 2;
        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(validSecret);

        A.CallTo(() => _secretStore.GetAccessHistory(secretId))
            .Returns(new List<AccessEvent>
            {
                new() { EventDate = new DateTime(2022, 05, 21)}
            });

        A.CallTo(() => _secretStore.CreateAccessEvent(A<AccessEvent>.Ignored, A<SecuredSecret>.That.Matches(e => e.Id == secretId)))
            .Returns(new AccessEvent());

        // Act
        var result = await _useCase.GetSecret(new GetSecretRequest
        {
            SecretId = secretId,
            Password = "Password1!"
        });

        // Assert
        result.Success.Should().BeTrue();
        A.CallTo(() => _secretStore.CreateAccessEvent(A<AccessEvent>.Ignored, A<SecuredSecret>.That.Matches(e => e.Id == secretId)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetSecret_MaxAccessCountHit_ReturnsInvalidRequest()
    {
        // Arrange 
        const string secretId = "secret-123";

        var validSecret = GetValidSecret();
        validSecret.Metadata.MaxAccessCount = 2;
        A.CallTo(() => _secretStore.GetSecret(secretId)).Returns(validSecret);

        A.CallTo(() => _secretStore.GetAccessHistory(secretId))
            .Returns(new List<AccessEvent>
            {
                new() { EventDate = new DateTime(2022, 05, 21)},
                new() { EventDate = new DateTime(2022, 05, 22)}
            });

        A.CallTo(() => _secretStore.CreateAccessEvent(A<AccessEvent>.Ignored, A<SecuredSecret>.That.Matches(e => e.Id == secretId)))
            .Returns(new AccessEvent());

        // Act
        var result = await _useCase.GetSecret(new GetSecretRequest
        {
            SecretId = secretId,
            Password = "Password1!"
        });

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ResultReason.InvalidRequest);
        A.CallTo(() => _secretStore.CreateAccessEvent(A<AccessEvent>.Ignored, A<SecuredSecret>.That.Matches(e => e.Id == secretId)))
            .MustNotHaveHappened();
    }

    private SecuredSecret GetValidSecret() => new()
    {
        Id = "secret-123",
        EncryptedValue = "Kds7J2jdOMleFTEYdkEHgQ==",
        InternalIV = "y0DsjWW7YCVKkhba5+qkgQ==",
        Metadata = new SecurityMetadata
        {
            Expiration = _utcNow.AddDays(2),
            Password = "qXM0MgTRwP+3QGQ6DqTgMoajKnCN9+MRehTk2QC1w1i6IsnGUUJowS1sQDvbhf8rt0Jvhl7epelK3MaRwsVlrA==",
            Salt = "AAA111"
        }
    };
}
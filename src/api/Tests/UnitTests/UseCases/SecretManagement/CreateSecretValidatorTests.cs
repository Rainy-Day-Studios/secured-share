using System;
using System.Linq;
using Entities;
using FakeItEasy;
using FluentAssertions;
using UseCases.Common;
using UseCases.SecretManagement;
using UseCases.SecretManagement.DTO;
using Xunit;

namespace Tests.UnitTests.UseCases.SecretManagement;

public class CreateSecretValidatorTests
{
    private readonly IDateTimeProvider _dtProvider = A.Fake<IDateTimeProvider>();
    private readonly DateTime _utcNow = new(2022, 05, 21);

    public CreateSecretValidatorTests()
    {
        A.CallTo(() => _dtProvider.UtcNow).Returns(_utcNow);
    }

    [Fact]
    public void Validate_ValidModel_ReturnsValid()
    {
        // Arrange
        var validator = new CreateSecretValidator(_dtProvider);

        var model = new SecuredSecret
        {
            EncryptedValue = "abc",
            ClientIV = "123",
            Metadata = new SecurityMetadata
            {
                Expiration = _utcNow.AddDays(7),
                MaxAccessCount = 5
            }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValidoutMaxAccessCount_ReturnsValid()
    {
        // Arrange
        var validator = new CreateSecretValidator(_dtProvider);

        var model = new SecuredSecret
        {
            EncryptedValue = "abc",
            ClientIV = "123",
            Metadata = new SecurityMetadata
            {
                Expiration = _utcNow.AddDays(7),
                MaxAccessCount = null
            }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Validate_EmptySecret_ReturnsInvalid(string secretVal)
    {
        // Arrange
        var validator = new CreateSecretValidator(_dtProvider);

        var model = new SecuredSecret
        {
            EncryptedValue = secretVal,
            ClientIV = "123",
            Metadata = new SecurityMetadata
            {
                Expiration = _utcNow.AddDays(7),
                MaxAccessCount = null
            }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Single().PropertyName.Should().Be(nameof(SecuredSecret.EncryptedValue));
    }

    [Fact]
    public void Validate_PastExpirationDate_ReturnsInvalid()
    {
        // Arrange
        var validator = new CreateSecretValidator(_dtProvider);

        var model = new SecuredSecret
        {
            EncryptedValue = "abc",
            ClientIV = "123",
            Metadata = new SecurityMetadata
            {
                Expiration = _utcNow.AddDays(-7),
                MaxAccessCount = null
            }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Single().PropertyName.Should().Be(nameof(SecuredSecret.Metadata) + "." + nameof(SecuredSecret.Metadata.Expiration));
    }

    [Fact]
    public void Validate_TodayExpirationDate_ReturnsInvalid()
    {
        // Arrange
        var validator = new CreateSecretValidator(_dtProvider);

        var model = new SecuredSecret
        {
            EncryptedValue = "abc",
            ClientIV = "123",
            Metadata = new SecurityMetadata
            {
                Expiration = _utcNow,
                MaxAccessCount = null
            }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Single().PropertyName.Should().Be(nameof(SecuredSecret.Metadata) + "." + nameof(SecuredSecret.Metadata.Expiration));
    }

    [Fact]
    public void Validate_NegativeAccessCount_ReturnsInvalid()
    {
        // Arrange
        var validator = new CreateSecretValidator(_dtProvider);

        var model = new SecuredSecret
        {
            EncryptedValue = "abc",
            ClientIV = "123",
            Metadata = new SecurityMetadata
            {
                Expiration = _utcNow.AddDays(7),
                MaxAccessCount = -1
            }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Single().PropertyName.Should().Be(nameof(SecuredSecret.Metadata) + "." + nameof(SecuredSecret.Metadata.MaxAccessCount));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Validate_NoClientIV_ReturnsInvalid(string clientIV)
    {
        // Arrange
        var validator = new CreateSecretValidator(_dtProvider);

        var model = new SecuredSecret
        {
            EncryptedValue = "abc",
            ClientIV = clientIV,
            Metadata = new SecurityMetadata
            {
                Expiration = _utcNow.AddDays(7)
            }
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Single().PropertyName.Should().Be(nameof(SecuredSecret.ClientIV));
    }
}
using System;
using Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests.Entities;

public class SecurityMetadataTests
{
    [Fact]
    public void IsExpired_ExpirationNotPassed_ReturnsFalse()
    {
        // Arrange
        var model = new SecurityMetadata { Expiration = new DateTime(2022, 05, 22) };

        // Act
        var isExpired = model.IsExpired(new DateTime(2022, 05, 21));

        // Assert
        isExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ExpirationPassed_ReturnsTrue()
    {
        // Arrange
        var model = new SecurityMetadata { Expiration = new DateTime(2022, 05, 22) };

        // Act
        var isExpired = model.IsExpired(new DateTime(2022, 05, 23));

        // Assert
        isExpired.Should().BeTrue();
    }
}
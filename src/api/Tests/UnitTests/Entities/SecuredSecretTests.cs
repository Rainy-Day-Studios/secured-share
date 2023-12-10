using System;
using System.Collections.Generic;
using Entities;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests.Entities;

public class SecuredSecretTests
{
    [Fact]
    public void MaxAccessCountExceeded_NoMaxAccessCountAndNoAccessHistory_ReturnsFalse()
    {
        // Arrange
        var model = new SecuredSecret
        {
            Metadata = new SecurityMetadata { MaxAccessCount = null }
        };

        // Act
        var accessCountExceeded = model.MaxAccessCountExceeded;

        // Assert
        accessCountExceeded.Should().BeFalse();
    }

    [Fact]
    public void MaxAccessCountExceeded_NoMaxAccessCountAndAccessHistory_ReturnsFalse()
    {
        // Arrange
        var model = new SecuredSecret
        {
            Metadata = new SecurityMetadata { MaxAccessCount = null },
            AccessHistory = new List<AccessEvent> { new() }
        };

        // Act
        var accessCountExceeded = model.MaxAccessCountExceeded;

        // Assert
        accessCountExceeded.Should().BeFalse();
    }

    [Fact]
    public void MaxAccessCountExceeded_MaxAccessCountAndNoAccessHistory_ReturnsFalse()
    {
        // Arrange
        var model = new SecuredSecret
        {
            Metadata = new SecurityMetadata { MaxAccessCount = 2 }
        };

        // Act
        var accessCountExceeded = model.MaxAccessCountExceeded;

        // Assert
        accessCountExceeded.Should().BeFalse();
    }

    [Fact]
    public void MaxAccessCountExceeded_MaxAccessCountAndAccessHistoryButNotExceeded_ReturnsFalse()
    {
        // Arrange
        var model = new SecuredSecret
        {
            Metadata = new SecurityMetadata { MaxAccessCount = 2 },
            AccessHistory = new List<AccessEvent> { new() }
        };

        // Act
        var accessCountExceeded = model.MaxAccessCountExceeded;

        // Assert
        accessCountExceeded.Should().BeFalse();
    }

    [Fact]
    public void MaxAccessCountExceeded_MaxAccessCountAndAccessHistoryMeetsLimit_ReturnsTrue()
    {
        // Arrange
        var model = new SecuredSecret
        {
            Metadata = new SecurityMetadata { MaxAccessCount = 2 },
            AccessHistory = new List<AccessEvent> { new(), new() }
        };

        // Act
        var accessCountExceeded = model.MaxAccessCountExceeded;

        // Assert
        accessCountExceeded.Should().BeTrue();
    }

    [Fact]
    public void MaxAccessCountExceeded_MaxAccessCountAndAccessHistoryExceededLimit_ReturnsTrue()
    {
        // Arrange
        var model = new SecuredSecret
        {
            Metadata = new SecurityMetadata { MaxAccessCount = 2 },
            AccessHistory = new List<AccessEvent> { new(), new(), new() }
        };

        // Act
        var accessCountExceeded = model.MaxAccessCountExceeded;

        // Assert
        accessCountExceeded.Should().BeTrue();
    }
}
using FluentAssertions;
using UseCases;
using Xunit;

namespace Tests.UnitTests.UseCases;

public class EncryptorTests
{
    [Fact]
    public void EncryptDecrypt_ValidKeyValue_ReturnsOriginalAfterEncryptAndDecrypt()
    {
        // Arrange
        const string key = "The_SecretKey_12";
        const string value = "my_secret1";

        // Act
        var encrypted = Encryptor.Encrypt(key, value);
        var decrypted = Encryptor.Decrypt(key, encrypted.IV, encrypted.Value);

        // Assert
        decrypted.Should().Be(value);
    }
}
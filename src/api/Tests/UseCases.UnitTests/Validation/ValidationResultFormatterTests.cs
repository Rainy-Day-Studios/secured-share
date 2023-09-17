using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.Results;
using UseCases.Validation;
using Xunit;

namespace Tests.UseCases.UnitTests.Validation;

public class ValidationResultFormatterTests
{
    [Fact]
    public void FormatValidationErrorMessage_OneError_FormatsCorrectly()
    {
        // Arrange
        const string propName = "prop1";
        const string errMsg = "err1";
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new() { PropertyName = propName, ErrorMessage = errMsg }
        });

        // Act
        var result = validationResult.FormatValidationErrorMessage();

        // Assert
        result.Should().Be($"Validation Failed. '{propName}': '{errMsg}'");
    }
}
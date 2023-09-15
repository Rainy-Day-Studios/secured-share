using FluentValidation.Results;

namespace UseCases.Validation;

public static class ValidationResultFormatter
{
    public static string FormatValidationErrorMessage(this ValidationResult result)
    {
        return 
            "Validation Failed. " + 
            $"{string.Join(", ", result.Errors.Select(e => $"'{e.PropertyName}': '{e.ErrorMessage}'"))}";
    }
}
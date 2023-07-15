using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Validation;

public class ValidationMethods
{
    public static ValidationResult MoreThanTwoDecimalPlaces(decimal value, ValidationContext context) 
    {
        bool result = decimal.Round(value, 2) != value;
        if (!result)
            return ValidationResult.Success;
        else
            return new ValidationResult(
                $"{context.MemberName} cannot have more than 2 decimal places.",
                new List<string>() { context.MemberName });
    }

    public static ValidationResult GreaterThanZero(decimal value, ValidationContext context)
    {
        if (value > 0)
            return ValidationResult.Success;
        else
            return new ValidationResult(
                $"{context.MemberName} must be positive.",
                new List<string>() { context.MemberName });
    }
}
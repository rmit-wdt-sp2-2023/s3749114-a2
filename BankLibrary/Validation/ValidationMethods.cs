﻿using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Validation;

public static class ValidationMethods
{
    public static ValidationResult MoreThanTwoDecimalPlaces(decimal value, ValidationContext context) 
    {
        bool result = decimal.Round(value, 2) != value;
        if (!result)
            return ValidationResult.Success;
        else
            return new ValidationResult("Can't have more than 2 decimals.", new List<string>() { context.MemberName });
    }

    public static ValidationResult GreaterThanZero(decimal value, ValidationContext context)
    {
        if (value > 0)
            return ValidationResult.Success;
        else
            return new ValidationResult("Must be a positive number.", new List<string>() { context.MemberName });
    }

    public static ValidationResult IsTenMinsFromNowUtc(DateTime? value, ValidationContext context)
    {
        if (value > DateTime.UtcNow.AddMinutes(10))
            return ValidationResult.Success;
        else
            return new ValidationResult(
                "Must be at least 10 minutes into the future.", new List<string>() { context.MemberName });  
    }

    public static ValidationResult IsTenMinsFromNowLocal(DateTime? value, ValidationContext context)
    {
        if (value > DateTime.Now.AddMinutes(10))
            return ValidationResult.Success;
        else
            return new ValidationResult(
                "Must be at least 10 minutes into the future.", new List<string>() { context.MemberName });
    }

    public static bool Validate<T>(T model, out List<ValidationResult> results)
    {
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(model, new ValidationContext(model), results, true);
    }
}

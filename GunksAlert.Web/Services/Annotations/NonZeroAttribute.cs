using System;
using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Services.Attributes;

public class NonZeroAttribute : ValidationAttribute {
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    ) {
        if (value is int intVal && intVal == 0) {
            return new ValidationResult("Value must not be zero");
        }

        return ValidationResult.Success;
    }
}

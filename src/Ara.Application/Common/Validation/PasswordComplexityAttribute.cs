using System.ComponentModel.DataAnnotations;

namespace Ara.Application.Common.Validation;

public class PasswordComplexityAttribute : ValidationAttribute
{
    public PasswordComplexityAttribute() : base(PasswordPolicy.Requirements)
    {
    }

    public override bool IsValid(object? value) => value is string password && PasswordPolicy.IsValid(password);
}

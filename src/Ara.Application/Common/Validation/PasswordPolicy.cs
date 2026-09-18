using System.Text.RegularExpressions;

namespace Ara.Application.Common.Validation;

/// <summary>
/// Validates a raw (pre-hash) password. Enforce this at registration/password-change time,
/// before the password is hashed — a hash itself can't carry these characteristics.
/// </summary>
public static partial class PasswordPolicy
{
    public const string Requirements =
        "Password must be at least 12 characters and include a number, an uppercase letter, and a special character.";

    public static bool IsValid(string password) => PasswordPattern().IsMatch(password);

    [GeneratedRegex(@"^(?=.*[0-9])(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{12,}$")]
    private static partial Regex PasswordPattern();
}

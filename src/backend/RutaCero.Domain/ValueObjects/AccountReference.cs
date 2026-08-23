using System.Text.RegularExpressions;
using RutaCero.Domain.Common;

namespace RutaCero.Domain.ValueObjects;

public readonly partial record struct AccountReference
{
    public string Value { get; }

    public AccountReference(string value)
    {
        var clean = Digits().Replace(value ?? string.Empty, string.Empty);
        if (clean.Length is < 2 or > 34)
            throw new DomainException("The account number must contain between 2 and 34 digits.");
        Value = clean;
    }

    [GeneratedRegex("[^0-9]")]
    private static partial Regex Digits();
}

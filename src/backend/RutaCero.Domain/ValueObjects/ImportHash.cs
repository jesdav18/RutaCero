using System.Text.RegularExpressions;using RutaCero.Domain.Common;
namespace RutaCero.Domain.ValueObjects;
public readonly partial record struct ImportHash
{
 public string Value{get;}public ImportHash(string value){var normalized=value??string.Empty;if(!HashPattern().IsMatch(normalized))throw new DomainException("Import hash must be SHA-256.");Value=normalized.ToUpperInvariant();}
 [GeneratedRegex("^[A-Fa-f0-9]{64}$")]private static partial Regex HashPattern();
}

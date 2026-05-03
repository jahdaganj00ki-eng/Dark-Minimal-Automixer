using System.Text.RegularExpressions;

namespace AutoDJMix.Domain.Music;

public readonly record struct CamelotKey(string Value)
{
    private static readonly Regex Pattern = new(@"^(?:[1-9]|1[0-2])[AB]$", RegexOptions.Compiled);

    public static CamelotKey Parse(string value)
    {
        if (!TryParse(value, out var key))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Camelot key.");
        }

        return key;
    }

    public static bool TryParse(string? value, out CamelotKey key)
    {
        if (value is null)
        {
            key = default;
            return false;
        }

        var normalized = value.Trim().ToUpperInvariant();
        if (!Pattern.IsMatch(normalized))
        {
            key = default;
            return false;
        }

        key = new CamelotKey(normalized);
        return true;
    }
}

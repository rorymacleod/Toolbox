using System.Security.Cryptography;
using System.Text;
using System;

namespace Toolbox;

public static class StringExtensions
{
    public const char Ellipsis = '…';

    /// <summary>
    /// Returns the SHA1 hash of the string.
    /// </summary>
    public static string ToHash(this string str)
    {
        using var crypto = HashAlgorithm.Create("SHA1") ??
            throw new InvalidOperationException("Hash algorithm for \"SHA1\" not found.");

        var hash = crypto.ComputeHash(Encoding.UTF8.GetBytes(str));
        var result = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            result.Append(b.ToString("x2"));
        }

        return result.ToString();
    }

    /// <summary>
    /// Removes one occurrence of the specified string if it appears at the end of the current string.
    /// </summary>
    public static string TrimEnd(this string current, string toRemove)
    {
        return current.TrimEnd(toRemove, StringComparison.CurrentCulture);
    }

    /// <summary>
    /// Removes one occurrence of the specified string if it appears at the end of the current string.
    /// </summary>
    public static string TrimEnd(this string current, string toRemove, StringComparison comparisonType)
    {
        if (current.EndsWith(toRemove, comparisonType))
        {
            return current.Substring(0, current.Length - toRemove.Length);
        }

        return current;
    }

    /// <summary>
    /// Returns the string shortened to the given length, including the optional suffix.
    /// </summary>
    public static string Truncate(this string str, int length, char? suffix = null)
    {
        if (length < 0)
            throw new ArgumentException("Must not be less than 0.", nameof(length));
        if (suffix.HasValue && length < 1)
            throw new ArgumentException("Must not be less than 1.", nameof(length));

        if (length >= str.Length)
            return str;

        return suffix.HasValue ? str.Substring(0, length - 1) + suffix.Value : str.Substring(0, length);
    }

    /// <summary>
    /// Returns the string shortened to the given length, including the optional suffix.
    /// </summary>
    public static string Truncate(this string str, int length, string tail)
    {
        if (length < 0)
            throw new ArgumentException("Must not be less than 0.", nameof(length));
        if (tail != null && length < tail.Length)
            throw new ArgumentException($"'{nameof(length)}' must not be less than the length of '{nameof(tail)}'.",
                nameof(length));

        if (length >= str.Length)
            return str;

        return tail != null ? string.Concat(str.AsSpan(0, length - tail.Length), tail) : str.Substring(0, length);
    }
}

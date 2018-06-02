using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Toolbox
{
    /// <summary>
    /// Adds extensions methods to <see cref="string"/> objects.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// A single-character ellipsis.
        /// </summary>
        public const char Ellipsis = '…';



        /// <summary>
        /// Returns the SHA1 hash of the string.
        /// </summary>
        /// <param name="str">The current <see cref="string"/>.</param>
        public static string ToHash(this string str)
        {
            if (str == null)
                throw new NullReferenceException();

            using (var crypto = new SHA1Managed())
            {
                var hash = crypto.ComputeHash(Encoding.UTF8.GetBytes(str));
                var result = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// Removes one occurance of the specified string if it appears at the end of the current string.
        /// </summary>
        /// <param name="current">The current string.</param>
        /// <param name="tail">The <see cref="string"/> to remove.</param>
        public static string TrimEnd(this string current, string tail)
        {
            return current.TrimEnd(tail, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Removes one occurance of the specified string if it appears at the end of the current string.
        /// </summary>
        /// <param name="current">The current string.</param>
        /// <param name="tail">The <see cref="string"/> to remove.</param>
        /// <param name="comparisonType">Determines how the strings are compared.</param>
        public static string TrimEnd(this string current, string tail, StringComparison comparisonType)
        {
            if (current == null)
                throw new ArgumentNullException(nameof(current));

            if (current.EndsWith(tail, comparisonType))
            {
                return current.Substring(0, current.Length - tail.Length);
            }

            return current;
        }

        /// <summary>
        /// Returns the string shortened to the given length, including the optional suffix.
        /// </summary>
        /// <param name="str">The current <see cref="string"/>.</param>
        /// <param name="length">The maximum length of the returned string.</param>
        /// <param name="suffix">Optional <see cref="char"/> added to the end of the string. If specified, 
        /// <paramref name="length"/> includes the suffix.</param>
        public static string Truncate(this string str, int length, char? suffix = null)
        {
            if (str == null)
                throw new NullReferenceException();
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
        /// <param name="str">The current <see cref="string"/>.</param>
        /// <param name="length">The maximum length of the returned string.</param>
        /// <param name="tail">Optional <see cref="string"/> added to the end of the string. If specified, 
        /// <paramref name="length"/> includes the suffix.</param>
        public static string Truncate(this string str, int length, string tail)
        {
            if (str == null)
                throw new NullReferenceException();
            if (length < 0)
                throw new ArgumentException("Must not be less than 0.", nameof(length));
            if (tail != null && length < tail.Length)
                throw new ArgumentException($"'{nameof(length)}' must not be less than the legnth of '{nameof(tail)}'.", 
                    nameof(length));

            if (length >= str.Length)
                return str;

            return tail != null ? str.Substring(0, length - tail.Length) + tail : str.Substring(0, length);
        }
        
                /// <summary>
        /// Breaks the current string into one or more lines of a specified maximum length, wrapping on word breaks.
        /// </summary>
        /// <param name="str">The current <see cref="string"/>.</param>
        /// <param name="lineLength">The maximum length of the returned lines.</param>
        /// <returns>A sequence of the lines read from the original string.</returns>
        public static IEnumerable<string> WrapLines(this string str, int lineLength)
        {
            if (string.IsNullOrEmpty(str))
                yield break;

            var whitespaceChars = new[] { ' ', '\t', '\r', '\n' };
            int idx = 0;
            while (idx < str.Length && str.Length - idx > lineLength)
            {
                int end = str.LastIndexOfAny(whitespaceChars, idx + lineLength + 1, lineLength);
                if (end > -1)
                {
                    yield return str.Substring(idx, end - idx);
                    idx = end;
                }

                yield return str.Substring(idx, end - 1) + '-';
                idx = end - 1;
            }

            if (idx < str.Length - 1)
            {
                yield return str.Substring(idx);
            }
        }

    }
}

using System;
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
    }
}

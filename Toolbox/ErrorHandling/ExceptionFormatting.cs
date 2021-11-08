using System;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Indicates how to format error messages derived from a series of exceptions.
    /// </summary>
    public enum ExceptionFormatting
    {
        /// <summary>
        /// Error messages are concatenated into a single line.
        /// </summary>
        SingleLine = 0,

        /// <summary>
        /// The error message from each nested exception is presented on its own line.
        /// </summary>
        Multiline,

        /// <summary>
        /// Error messages are presented on their own line, followed by an indented list of the values from the
        /// exception's properties.
        /// </summary>
        Detailed
    }
}

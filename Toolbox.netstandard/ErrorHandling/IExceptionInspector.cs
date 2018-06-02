using System;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Provides analysis of an exception's properties and messages.
    /// </summary>
    public interface IExceptionInspector
    {
        /// <summary>
        /// Gets a detailed error message built from the exception's own message and its properties.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        /// <param name="formatting">Optional <see cref="ExceptionFormatting"/> value specifying how to format the
        /// error message.</param>
        string GetMessage(Exception exception, ExceptionFormatting formatting = ExceptionFormatting.SingleLine);

        /// <summary>
        /// Indicates whether the exception contains significant information, meaning that it is not a wrapper or
        /// aggregate of other exceptions.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        bool IsSignificant(Exception exception);

        /// <summary>
        /// Indicates whether the exception represents a transient error, meaning that the operation may succeed if it
        /// is retried.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        bool IsTransient(Exception exception);
    }
}

using System;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Abstract base class for <see cref="IExceptionInspector"/> implementations that support being chained together.
    /// </summary>
    public abstract class ExceptionInspector : IExceptionInspector
    {
        /// <summary>
        /// Gets a default <see cref="IExceptionInspector"/> that provides analysis of common .NET Framework exceptions.
        /// </summary>
        public static IExceptionInspector Default { get; } = new DefaultExceptionInspector();

        /// <summary>
        /// Synchronizes access from concurrent threads.
        /// </summary>
        private static readonly object SyncRoot = new object();




        /// <summary>
        /// Gets the next <see cref="IExceptionInspector"/> in the chain.
        /// </summary>
        public IExceptionInspector Next { get; private set; }




        /// <summary>
        /// Adds a new <see cref="IExceptionInspector"/> to the end of the chain.
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns>The current <see cref="IExceptionInspector"/>.</returns>
        /// <exception cref="InvalidOperationException">The last <see cref="IExceptionInspector"/> in the chain does
        /// is not derived from <see cref="ExceptionInspector"/>.</exception>
        public ExceptionInspector Append(IExceptionInspector inspector)
        {
            if (inspector == null)
                throw new ArgumentNullException(nameof(inspector));

            lock (SyncRoot)
            {
                if (Next == null)
                {
                    Next = inspector;
                }
                else if (Next is ExceptionInspector chain)
                {
                    chain.Append(inspector);
                }
                else
                {
                    throw new InvalidOperationException("The last IExceptionInspector in the chain is not an " +
                        "ExceptionInspector and does not support adding another inspector.");
                }
            }

            return this;
        }

        /// <summary>
        /// Gets a detailed error message built from the exception's own message and its properties.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        /// <param name="formatting">Optional <see cref="ExceptionFormatting"/> value specifying how to format the
        /// error message.</param>
        public abstract string GetMessage(Exception exception, ExceptionFormatting formatting = ExceptionFormatting.SingleLine);

        /// <summary>
        /// Indicates whether the exception contains significant information, meaning that it is not a wrapper or
        /// aggregate of other exceptions.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        public abstract bool IsSignificant(Exception exception);

        /// <summary>
        /// Indicates whether the exception represents a transient error, meaning that the operation may succeed if it
        /// is retried.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        public abstract bool IsTransient(Exception exception);
    }
}

using System;
using System.Collections.Generic;

namespace Toolbox
{
    /// <summary>
    /// Adds methods to <see cref="Exception"/> object.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Returns all of the exceptions in the inner-exception hierarchy as a single flat collection.
        /// </summary>
        /// <param name="exception">The current exception.</param>
        public static IEnumerable<Exception> Flatten(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            yield return exception;

            if (exception is AggregateException aggEx)
            {
                foreach (var ex in aggEx.InnerExceptions)
                {
                    foreach (var ex2 in ex.Flatten())
                    {
                        yield return ex2;
                    }
                }
            }
            else
            {
                if (exception.InnerException != null)
                {
                    foreach (var ex in exception.InnerException.Flatten())
                    {
                        yield return ex;
                    }
                }
            }
        }
    }
}

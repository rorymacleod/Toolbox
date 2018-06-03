using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Adds extension methods to <see cref="IExceptionInspector"/> objects.
    /// </summary>
    public static class ExceptionInspectorExtensions
    {
        /// <summary>
        /// Returns a dictionary containing data from the exception that can be used to build a detailed error message.
        /// </summary>
        /// <param name="inspector">The current <see cref="IExceptionInspector"/>.</param>
        /// <param name="exception">The exception to inspect.</param>
        public static SortedDictionary<string, string> GetData(this IExceptionInspector inspector, Exception exception)
        {
            var data = new SortedDictionary<string, string>();
            inspector.GetData(exception, data);
            return data;
        }

        /// <summary>
        /// Gets all error messages from the significant exceptions reachable from the given exception combined into a
        /// single message.
        /// </summary>
        /// <param name="inspector">The current <see cref="IExceptionInspector"/>.</param>
        /// <param name="exception">The exception to inspect.</param>
        /// <param name="formatting">Optional <see cref="ExceptionFormatting"/> value specifying how to format the
        /// error message.</param>
        public static string GetAllMessages(this IExceptionInspector inspector, Exception exception,
            ExceptionFormatting formatting = ExceptionFormatting.SingleLine)
        {
            var sb = new StringBuilder();
            foreach (var ex in exception.Flatten())
            {
                // Special handling of AggregateException - the number of errors is added to the output, and an error
                // number is added to the start of each line (or each error for SingleLine).
                if (ex is AggregateException aggEx && aggEx.InnerExceptions.Count > 1)
                {
                    if (formatting == ExceptionFormatting.SingleLine)
                    {
                        // Example: "2 errors occurred in parallel: (1) Alpha. (2) Bravo. Charlie."
                        sb.Append($"{aggEx.InnerExceptions.Count} errors occurred in parallel: ");
                    }
                    else
                    {
                        // Example:
                        // 2 errors occurred in parallel:
                        // (1) Alpha.
                        //     - Data
                        // (2) Bravo.
                        //     Charlie.
                        //     - Data
                        sb.AppendLine($"{aggEx.InnerExceptions.Count} errors occurred in parallel:");
                    }

                    for(int i = 1; i <= aggEx.InnerExceptions.Count; i++)
                    {
                        var innerEx = aggEx.InnerExceptions[i - 1];
                        var innerMsg = inspector.GetAllMessages(innerEx, formatting);

                        if (formatting == ExceptionFormatting.SingleLine)
                        {
                            sb.Append($" ({i}) {innerMsg}");
                        }
                        else
                        {
                            sb.AppendLine($"{i.ToString().PadLeft(2)}| " +
                                innerMsg.Replace(Environment.NewLine, Environment.NewLine + "    "));
                        }
                    }
                    // If this is an AggregateException, the we've already handled all the inner exceptions; the rest
                    // of the output from Flatten does not matter.
                    break;
                }

                if (inspector.IsSignificant(ex))
                {
                    sb.Append(inspector.GetMessage(ex, formatting));
                    if (formatting == ExceptionFormatting.SingleLine)
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Gets a detailed error message built from the exception's own message and its properties.
        /// </summary>
        /// <param name="inspector">The current <see cref="IExceptionInspector"/>.</param>
        /// <param name="exception">The exception to inspect.</param>
        /// <param name="formatting">Optional <see cref="ExceptionFormatting"/> value specifying how to format the
        /// error message.</param>
        public static string GetMessage(this IExceptionInspector inspector, Exception exception,
            ExceptionFormatting formatting = ExceptionFormatting.SingleLine)
        {
            IDictionary<string, string> data = null;
            if (formatting == ExceptionFormatting.Detailed)
            {
                data = inspector.GetData(exception);
            }

            return inspector.GetMessage(exception, data, formatting);
        }

        /// <summary>
        /// Gets a collection of only the significant exception reachable from the given exception.
        /// </summary>
        /// <param name="inspector">The current <see cref="IExceptionInspector"/>.</param>
        /// <param name="exception">The exception to inspect.</param>
        public static IEnumerable<Exception> GetSignificant(this IExceptionInspector inspector, Exception exception)
        {
            foreach (var ex in exception.Flatten())
            {
                if (inspector.IsSignificant(ex))
                    yield return ex;
            }
        }
    }
}

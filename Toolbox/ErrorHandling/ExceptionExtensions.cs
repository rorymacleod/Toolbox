using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.ErrorHandling;

/// <summary>
/// Adds methods to <see cref="Exception"/> object.
/// </summary>
public static class ExceptionExtensions
{
    private static void AppendParallelMessages(ExceptionFormatting formatting, StringBuilder sb, AggregateException aggEx)
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

        for (int exNum = 1; exNum <= aggEx.InnerExceptions.Count; exNum++)
        {
            var innerEx = aggEx.InnerExceptions[exNum - 1];
            var innerMsg = GetAllMessages(innerEx, formatting);

            if (formatting == ExceptionFormatting.SingleLine)
            {
                sb.Append($" ({exNum}) {innerMsg}");
            }
            else
            {
                sb.AppendLine($"{exNum,2}| " +
                    innerMsg.Replace(Environment.NewLine, Environment.NewLine + "    "));
            }
        }
    }

    /// <summary>
    /// Returns the exception and all of the exceptions in the inner-exception hierarchy as a single flat collection.
    /// </summary>
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
        else if (exception.InnerException != null)
        {
            foreach (var ex in exception.InnerException.Flatten())
            {
                yield return ex;
            }
        }
    }

    /// <summary>
    /// Gets all error messages from the significant exceptions reachable from the given exception combined into a
    /// single message.
    /// </summary>
    public static string GetAllMessages(this Exception exception,
        ExceptionFormatting formatting = ExceptionFormatting.SingleLine)
    {
        var sb = new StringBuilder();
        foreach (var ex in exception.Flatten())
        {
            if (ex is AggregateException aggEx && aggEx.InnerExceptions.Count > 1)
            {
                AppendParallelMessages(formatting, sb, aggEx);
                break;
            }

            if (IsSignificant(ex))
            {
                sb.Append(GetMessage(ex, formatting));
                if (formatting == ExceptionFormatting.SingleLine)
                {
                    sb.Append(' ');
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
    /// Returns a dictionary containing data from the exception that can be used to build a detailed error message.
    /// </summary>
    public static SortedDictionary<string, string> GetData(this Exception exception)
    {
        var data = new SortedDictionary<string, string>();
        ExceptionInspector.Get(typeof(Exception)).GetData(exception, data);
        return data;
    }

    /// <summary>
    /// Gets a detailed error message built from the exception's own message and its properties.
    /// </summary>
    public static string GetMessage(this Exception exception,
        ExceptionFormatting formatting = ExceptionFormatting.SingleLine)
    {
        var inspector = ExceptionInspector.Get(exception.GetType());
        if (formatting == ExceptionFormatting.Detailed)
        {
            return inspector.GetMessage(exception, GetData(exception), formatting);
        }

        return inspector.GetMessage(exception, null, formatting);
    }

    /// <summary>
    /// Returns a collection of only the significant exceptions reachable from the given exception.
    /// </summary>
    public static IEnumerable<Exception> GetSignificant(this Exception exception)
    {
        foreach (var ex in exception.Flatten())
        {
            if (IsSignificant(ex))
                yield return ex;
        }
    }

    /// <summary>
    /// Indicates whether the exception contains significant information, meaning that it is not a wrapper or
    /// aggregate of other exceptions.
    /// </summary>
    public static bool IsSignificant(this Exception exception)
    {
        return ExceptionInspector.Get(exception.GetType()).IsSignificant(exception);
    }

    /// <summary>
    /// Indicates whether the exception represents a transient error, meaning that the operation may succeed if it
    /// is retried.
    /// </summary>
    public static bool IsTransient(this Exception exception)
    {
        return ExceptionInspector.Get(exception.GetType()).IsTransient(exception);
    }
}

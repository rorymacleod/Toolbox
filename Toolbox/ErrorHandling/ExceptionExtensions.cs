using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Toolbox.Collections;

namespace Toolbox.ErrorHandling;

/// <summary>
/// Adds methods to <see cref="Exception"/> object.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Returns the exception and all of the exceptions in the inner-exception hierarchy as a single flat collection.
    /// </summary>
    public static IEnumerable<Exception> Flatten(this Exception exception)
    {
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
    public static string GetAllMessages(this Exception exception, bool multiline = false, bool includeData = false)
    {
        var sb = new StringBuilder();
        foreach (var ex in exception.Flatten())
        {
            if (ex is AggregateException aggEx && aggEx.InnerExceptions.Count > 1)
            {
                AppendParallelMessages(sb, aggEx, multiline, includeData);
                break;
            }

            if (IsSignificant(ex) || (ex.InnerException == null && sb.Length == 0))
            {
                sb.Append(GetMessage(ex, multiline, includeData));
                if (multiline)
                {
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(' ');
                }
            }
        }

        return sb.ToString().TrimEnd();
    }


    /// <summary>
    /// Returns a dictionary containing data from the exception that can be used to build a detailed error message.
    /// </summary>
    public static SortedDictionary<string, string?> GetData(this Exception exception)
    {
        var data = new SortedDictionary<string, string?>();
        foreach (string key in exception.Data.Keys)
        {
            object? value = exception.Data[key];
            data[key] = value == null ? null : Convert.ToString(value);
        }

        return data;
    }

    /// <summary>
    /// Gets a detailed error message built from the exception's own message and its properties.
    /// </summary>
    public static string GetMessage(this Exception exception, bool multiline = false, bool includeData = false)
    {
        var sb = new StringBuilder();
        if (multiline)
        {
            AppendExceptionName(sb, exception);
            AppendMessage(sb, exception);
        }
        else
        {
            AppendMessage(sb, exception);
        }

        if (includeData)
        {
            AppendAllData(sb, exception, multiline);
        }

        return sb.ToString();
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
        switch (exception.GetType().FullName)
        {
            case "System.AggregateException":
            case "System.Reflection.TargetInvocationException":
                return false;

            default:
                return true;
        }
    }

    /// <summary>
    /// Indicates whether the exception represents a transient error, meaning that the operation may succeed if it
    /// is retried.
    /// </summary>
    public static bool IsTransient(this Exception exception)
    {
        switch (exception.GetType().FullName)
        {
            case "System.Threading.ThreadAbortException":
            case "System.OperationCancelledException":
            case "System.Threading.Tasks.TaskCanceledException":
            case "System.TimeoutException":
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Populates the exception's <see cref="Exception.Data"/> dictionary from the given dictionary.
    /// </summary>
    public static T SetData<T>(this T exception, IDictionary data) where T : Exception
    {
        foreach (var key in data.Keys)
        {
            exception.Data[key] = data[key];
        }

        return exception;
    }

    /// <summary>
    /// Populates the exception's <see cref="Exception.Data"/> dictionary from the public properties of the given object.
    /// </summary>
    public static T SetData<T>(this T exception, object? data) where T : Exception
    {
        DictionaryHelper.AddObjectTo(exception.Data, data);
        return exception;
    }

    /// <summary>
    /// Adds the given key-value pair to the exception's <see cref="Exception.Data"/> dictionary.
    /// </summary>
    public static T SetData<T>(this T exception, string key, object? value) where T : Exception
    {
        exception.Data[key] = value;
        return exception;
    }

    private static void AppendAllData(StringBuilder sb, Exception exception, bool multiline)
    {
        var data = exception.GetData();
        if (data.Count == 0)
            return;

        if (multiline)
        {
            sb.AppendLine();
        }
        else
        {
            if (sb[^1] == '.')
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(" (");
        }

        int i = 0;
        foreach (var (key, value) in data)
        {
            sb.Append(multiline ? $"- {key}" : key);
            sb.Append($"=[{value ?? "<null>"}]");
            if (++i < data.Count)
            {
                if (multiline) {
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(", ");
                }
            }
        }

        if (!multiline)
        {
            sb.Append(").");
        }
    }

    private static void AppendExceptionName(StringBuilder sb, Exception exception)
    {
        var exType = exception.GetType();
        var name = exType == typeof(Exception) ? exType.Name : exType.Name.TrimEnd("Exception",
            StringComparison.InvariantCultureIgnoreCase);
        sb.AppendFormat("({0}) ", name);
    }

    private static void AppendMessage(StringBuilder sb, Exception exception)
    {
        sb.Append(exception.Message);
    }

    private static void AppendParallelMessages(StringBuilder sb, AggregateException aggEx, bool multiline, bool includeData)
    {
        if (multiline)
        {
            // Example:
            // 2 errors occurred in parallel:
            //  1| Alpha.
            //     - Data
            //  2| Bravo.
            //     Charlie.
            //     - Data
            sb.AppendLine($"{aggEx.InnerExceptions.Count} errors occurred in parallel:");
        }
        else
        {
            // Example: "2 errors occurred in parallel: (1) Alpha. (2) Bravo. Charlie."
            sb.Append($"{aggEx.InnerExceptions.Count} errors occurred in parallel: ");
        }

        for (int exNum = 1; exNum <= aggEx.InnerExceptions.Count; exNum++)
        {
            var innerEx = aggEx.InnerExceptions[exNum - 1];
            var innerMsg = GetAllMessages(innerEx, multiline, includeData);

            if (multiline)
            {
                sb.AppendLine($"{exNum,2}| " +
                    innerMsg.Replace(Environment.NewLine, Environment.NewLine + "    "));
            }
            else
            {
                sb.Append($" ({exNum}) {innerMsg}");
            }
        }
    }
}

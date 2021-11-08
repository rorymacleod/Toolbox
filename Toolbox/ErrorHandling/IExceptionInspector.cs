using System;
using System.Collections.Generic;

namespace Toolbox.ErrorHandling;

/// <summary>
/// Provides analysis of an exception's properties and messages.
/// </summary>
public interface IExceptionInspector
{
    /// <summary>
    /// Populates the given dictionary with data that can be used to build a detailed error message.
    /// </summary>
    void GetData(Exception exception, IDictionary<string, string> data);

    /// <summary>
    /// Gets a detailed error message built from the exception's own message and its properties.
    /// </summary>
    string GetMessage(Exception exception, IDictionary<string, string>? data,
        ExceptionFormatting formatting = ExceptionFormatting.SingleLine);

    /// <summary>
    /// Indicates whether the exception contains significant information, meaning that it is not a wrapper or
    /// aggregate of other exceptions.
    /// </summary>
    bool IsSignificant(Exception exception);

    /// <summary>
    /// Indicates whether the exception represents a transient error, meaning that the operation may succeed if it
    /// is retried.
    /// </summary>
    bool IsTransient(Exception exception);
}
using System;
using System.Collections;
using System.Runtime.Serialization;
using Toolbox.Collections;

namespace Toolbox;

/// <summary>
/// The exception that is thrown when an error occurs in the application.
/// </summary>
[Serializable]
public class AppException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class with the specified error message.
    /// </summary>
    public AppException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class with the specified error message 
    /// and a reference to the inner exception that caused the error.
    /// </summary>
    public AppException(string message, Exception innerException) :
        base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class from serialized data.
    /// </summary>
    protected AppException(SerializationInfo info, StreamingContext context)
          : base(info, context)
    {
    }

    /// <summary>
    /// Populates the exception's <see cref="Exception.Data"/> dictionary from the given dictionary.
    /// </summary>
    /// <param name="data">The dictionary to copy to <see cref="Exception.Data"/>.</param>
    public AppException SetData(IDictionary data)
    {
        foreach (var key in data.Keys)
        {
            Data[key] = data[key];
        }

        return this;
    }

    /// <summary>
    /// Populates the exception's <see cref="Exception.Data"/> dictionary from the public properties of the given object.
    /// </summary>
    public AppException SetData(object? data)
    {
        DictionaryHelper.AddObjectTo(Data, data);
        return this;
    }

    /// <summary>
    /// Adds the given key-value pair to the exception's <see cref="Exception.Data"/> dictionary.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The current <see cref="AppException"/> instance.</returns>
    public AppException SetData(string key, object? value)
    {
        Data[key] = value;
        return this;
    }
}

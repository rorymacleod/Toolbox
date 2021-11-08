using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolbox.Collections;

/// <summary>
/// Provides helper mothods for working with dictionaries.
/// </summary>
public static class DictionaryHelper
{
    /// <summary>
    /// Adds the properties and values from the given object to the dictionary.
    /// </summary>
    public static void AddObjectTo(IDictionary<string, string?> dictionary, object? obj)
    {
        if (obj == null)
            return;

        if (obj is IDictionary<string, string?> source)
        {
            foreach (var key in source.Keys)
            {
                dictionary[key] = source[key];
            }
            return;
        }
        else
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                object? value = property.GetValue(obj);
                dictionary[property.Name] = Convert.ToString(value);
            }
        }

    }

    /// <summary>
    /// Adds the properties and values from the given object to the dictionary.
    /// </summary>
    public static void AddObjectTo(IDictionary<string, object?> dictionary, object? obj)
    {
        if (obj == null)
            return;

        if (obj is IDictionary<string, object?> source)
        {
            foreach (var key in source.Keys)
            {
                dictionary[key] = source[key];
            }
        }
        else
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                object? value = property.GetValue(obj);
                dictionary[property.Name] = value;
            }
        }

    }

    /// <summary>
    /// Adds the properties and values from the given object to the dictionary.
    /// </summary>
    public static void AddObjectTo(IDictionary dictionary, object? obj)
    {
        if (obj == null)
            return;

        if (obj is IDictionary source)
        {
            foreach (var key in source.Keys)
            {
                dictionary[key] = source[key];
            }
        }
        else
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                object? value = property.GetValue(obj);
                dictionary[property.Name] = value;
            }
        }
    }
}

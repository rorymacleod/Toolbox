using System;
using System.Collections;
using System.Collections.Generic;
using Toolbox.Reflection;

namespace Toolbox.Collections
{
    /// <summary>
    /// Provides helper mothods for working with dictionaries.
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// Adds the properties and values from the given object to the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to populate.</param>
        /// <param name="obj">The object to read from, or, an existing <see cref="IDictionary{TKey, TValue}"/> of
        /// (<c>string</c>, <c>string</c>) to copy from.</param>
        /// <param name="reader">Optional <see cref="IObjectReader"/> used to read the contents of the object.</param>
        public static void AddObjectTo(IDictionary<string, string> dictionary, object obj, IObjectReader reader = null)
        {
            if (obj == null)
                return;

            if (obj is IDictionary<string, string> source)
            {
                foreach (var key in source.Keys)
                {
                    dictionary[key] = source[key];
                }
                return;
            }

            reader = reader ?? new ReflectionObjectReader();
            foreach (var pair in reader.GetValues(obj))
            {
                object value = pair.Value;
                dictionary[pair.Key] = value == null ? null : Convert.ToString(pair.Value);
            }
        }

        /// <summary>
        /// Adds the properties and values from the given object to the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to populate.</param>
        /// <param name="obj">The object to read from, or, an existing <see cref="IDictionary{TKey, TValue}"/> of
        /// (<c>string</c>, <c>object</c>) to copy from.</param>
        /// <param name="reader">Optional <see cref="IObjectReader"/> used to read the contents of the object.</param>
        public static void AddObjectTo(IDictionary<string, object> dictionary, object obj, IObjectReader reader = null)
        {
            if (obj == null)
                return;

            if (obj is IDictionary<string, object> source)
            {
                foreach (var key in source.Keys)
                {
                    dictionary[key] = source[key];
                }
                return;
            }

            reader = reader ?? new ReflectionObjectReader();
            foreach (var pair in reader.GetValues(obj))
            {
                dictionary[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Adds the properties and values from the given object to the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to populate.</param>
        /// <param name="obj">The object to read from, or, an existing <see cref="IDictionary"/> to copy from.</param>
        /// <param name="reader">Optional <see cref="IObjectReader"/> used to read the contents of the object.</param>
        public static void AddObjectTo(IDictionary dictionary, object obj, IObjectReader reader = null)
        {
            if (obj == null)
                return;

            if (obj is IDictionary source)
            {
                foreach (var key in source.Keys)
                {
                    dictionary[key] = source[key];
                }
                return;
            }

            reader = reader ?? new ReflectionObjectReader();
            foreach (var pair in reader.GetValues(obj))
            {
                dictionary[pair.Key] = pair.Value;
            }
        }
    }
}

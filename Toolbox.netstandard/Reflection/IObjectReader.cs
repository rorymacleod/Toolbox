using System;
using System.Collections.Generic;

namespace Toolbox.Reflection
{
    /// <summary>
    /// Defines a utility that reads the properties of an arbitrary object at runtime.
    /// </summary>
    public interface IObjectReader
    {
        /// <summary>
        /// Gets the value of the specified public property.
        /// </summary>
        /// <param name="target">The <see cref="object"/> to read from.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the specified property, or a null-reference if the property does not exist or is not
        /// accessible.</returns>
        object Get(object target, string name);

        /// <summary>
        /// Gets a collection of public property names exposed by the target object.
        /// </summary>
        /// <param name="target">The <see cref="object"/> to read from.</param>
        IEnumerable<string> GetNames(object target);

        /// <summary>
        /// Gets a collection of the public property names and values from the target object.
        /// </summary>
        /// <param name="target">The <see cref="object"/> to read from.</param>
        IEnumerable<KeyValuePair<string, object>> GetValues(object target);
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Toolbox.Reflection
{
    /// <summary>
    /// Reads the contents of an object using reflection.
    /// </summary>
    /// <remarks>
    /// This class is safe for use by concurrent threads.
    /// </remarks>
    public class ReflectionObjectReader : IObjectReader
    {
        /// <summary>
        /// A cache of the public properties of each object, by object type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ICollection<PropertyInfo>> PropertyCache = 
            new ConcurrentDictionary<Type, ICollection<PropertyInfo>>();


        /// <summary>
        /// Gets the value of the specified public property.
        /// </summary>
        /// <param name="target">The <see cref="object"/> to read from.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the specified property, or a null-reference if the property does not exist or is not
        /// accessible.</returns>
        public object Get(object target, string name)
        {
            if (target == null)
                return null;

            // Testing shows that calling GetProperty every time is faster than searching through the cached list.
            var type = target.GetType();
            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            return prop?.GetValue(target);
        }

        /// <summary>
        /// Gets a collection of public property names exposed by the target object.
        /// </summary>
        /// <param name="target">The <see cref="object"/> to read from.</param>
        public IEnumerable<string> GetNames(object target)
        {
            if (target == null)
                yield break;

            foreach (PropertyInfo prop in GetProperties(target))
            {
                yield return prop.Name;
            }
        }

        /// <summary>
        /// Gets the public properties of the given object.
        /// </summary>
        private ICollection<PropertyInfo> GetProperties(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var type = target.GetType();
            var props = PropertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            return props;
        }

        /// <summary>
        /// Gets a collection of the public property names and values from the target object.
        /// </summary>
        /// <param name="target">The <see cref="object"/> to read from.</param>
        public IEnumerable<KeyValuePair<string, object>> GetValues(object target)
        {
            foreach (PropertyInfo prop in GetProperties(target))
            {
                yield return new KeyValuePair<string, object>(prop.Name, prop.GetValue(target));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Toolbox.Logging
{
    /// <summary>
    /// Provides helper methods for creating consistent <see cref="EventId"/> values.
    /// </summary>
    public static class EventIdFactory
    {
        /// <summary>
        /// Returns an <see cref="int"/> derived from the given type that is a multiple of 100.
        /// </summary>
        /// <typeparam name="T">The type to derive a number from.</typeparam>
        public static int GetBaseId<T>()
        {
            return GetBaseId(typeof(T));
        }

        /// <summary>
        /// Returns an <see cref="int"/> derived from the given type that is a multiple of 100.
        /// </summary>
        /// <param name="type">The type to derive a number from.</param>
        public static int GetBaseId(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            using (var crypto = new SHA1Managed())
            {
                byte[] hash = crypto.ComputeHash(Encoding.UTF8.GetBytes(type.FullName));
                short num = BitConverter.ToInt16(hash, 0);

                return num * 100;
            }
        }
    }
}

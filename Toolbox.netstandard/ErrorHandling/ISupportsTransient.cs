using System;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Indicates that an exception explicitly supports an <c>IsTransient</c> property.
    /// </summary>
    public interface ISupportsTransient
    {
        /// <summary>
        /// Indicates whether the exception represents a transient error, meaning that the operation may succeed if it
        /// is retried.
        /// </summary>
        bool IsTransient { get; }
    }
}

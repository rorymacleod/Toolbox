using System;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Exposes a default <see cref="IExceptionInspector"/> that provides analysis of an exception's properties and
    /// messages.
    /// </summary>
    public static class ExceptionInspector
    {
        /// <summary>
        /// Gets a singleton instance of the default <see cref="IExceptionInspector"/>.
        /// </summary>
        public static IExceptionInspector Default { get; } = new DefaultExceptionInspector();
    }
}

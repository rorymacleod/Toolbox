using System;

namespace Toolbox
{
    /// <summary>
    /// Implements an object that calls an arbitrary <see cref="System.Action"/> whe it is disposed.
    /// </summary>
    public class Disposer: IDisposable
    {
        /// <summary>
        /// The <c>Action</c> to invoke when this object is disposed.
        /// </summary>
        private readonly Action Action;

        /// <summary>
        /// Initialized a new instance of the <see cref="Disposer"/> class.
        /// </summary>
        /// <param name="action">The <see cref="System.Action"/> to invoke when this object is disposed.</param>
        public Disposer(Action action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Action();
        }
    }
}

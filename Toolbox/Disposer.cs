using System;

namespace Toolbox;

/// <summary>
/// An object that invokes an arbitrary <see cref="System.Action"/> when it is disposed.
/// </summary>
public sealed class Disposer : IDisposable
{
    private readonly Action Action;

    public Disposer(Action action)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public void Dispose()
    {
        Action();
    }
}

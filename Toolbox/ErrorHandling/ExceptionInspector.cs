using System;
using System.Collections.Generic;

namespace Toolbox.ErrorHandling;

/// <summary>
/// Provides access to an <see cref="IExceptionInspector"/> for a given exception type.
/// </summary>
public static class ExceptionInspector
{
    private static readonly Dictionary<Type, IExceptionInspector> Inspectors = new() {
        { typeof(Exception), new DefaultExceptionInspector() }
    };

    public static IExceptionInspector Get(Type exceptionType)
    {
        lock (Inspectors)
        {
            IExceptionInspector inspector;
            while (!Inspectors.TryGetValue(exceptionType, out inspector!))
            {
                exceptionType = exceptionType.BaseType!;
            }

            return inspector;
        }
    }

    public static void Register(Type exceptionType, IExceptionInspector exceptionInspector)
    {
        lock (Inspectors)
        {
            Inspectors.Add(exceptionType, exceptionInspector);
        }
    }
}
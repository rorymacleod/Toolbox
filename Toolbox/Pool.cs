using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Toolbox;

public class Pool<T>
{
    private readonly TimeSpan Timeout;
    private readonly List<T> AvailableResources;
    private readonly AutoResetEvent WaitingForRelease = new(false);

    public Pool(IEnumerable<T> resources)
    {
        AvailableResources = new List<T>(resources);
    }

    public Pool(IEnumerable<T> resources, TimeSpan timeout) : this(resources)
    {
        Timeout = timeout;
    }

    public IDisposable Acquire(out T resource)
    {
        while (true)
        {
            lock (AvailableResources)
            {
                if (AvailableResources.Count > 0)
                {
                    resource = AvailableResources[0];
                    AvailableResources.RemoveAt(0);
                    T res = resource;
                    return new Disposer(() => Release(res));
                }
            }

            if (!WaitingForRelease.WaitOne(Timeout))
            {
                throw new TimeoutException("Unable to acquire pooled resource within allowed time.");
            }
        }
    }

    private void Release(T resource)
    {
        AvailableResources.Add(resource);
        WaitingForRelease.Set();
    }
}

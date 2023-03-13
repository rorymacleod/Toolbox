using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Toolbox.Tests;

public class PoolTests
{
    private static Pool<string> CreatePool(params string[] items)
    {
        var pool = new Pool<string>(items);
        return pool;
    }

    [Fact]
    public void Acquires_an_item_from_the_pool()
    {
        var pool = CreatePool("Alpha", "Bravo", "Charlie");
        
        pool.Acquire(out string item);
        
        item.Should().BeOneOf("Alpha", "Bravo", "Charlie");
    }

    [Fact]
    public void Does_not_return_an_item_already_acquired()
    {
        var pool = CreatePool("Alpha", "Bravo", "Charlie");

        pool.Acquire(out string item1);
        pool.Acquire(out string item2);
        pool.Acquire(out string item3);

        new[] { item1, item2, item3 }.Should().BeEquivalentTo("Alpha", "Bravo", "Charlie");
    }

    [Fact]
    public void Returns_an_acquired_item_that_has_been_released()
    {
        var pool = CreatePool("Alpha");
        var lease = pool.Acquire(out string _);
        lease.Dispose();

        pool.Acquire(out string item);

        item.Should().Be("Alpha");
    }

    [Fact]
    public async Task Waits_for_an_item_to_be_released_if_none_are_available()
    {
        var pool = CreatePool("Alpha");
        var lease = pool.Acquire(out string _);
        var operations = new List<string>();


        var task1 = Task.Run(() => {
            pool.Acquire(out string item);
            item.Should().Be("Alpha");

            lock (operations)
            {
                operations.Add("acquire");
            }
        });

        var task2 = Task.Delay(100).ContinueWith(t => {
            lease.Dispose();

            lock (operations)
            {
                operations.Add("release");
            }
        });

        Task.WaitAll(task1, task2);
        string.Join("-", operations).Should().Be("release-acquire");
    }

    /*
 * Returns an acquired item after it has been released
 * Waits for item to be released if none are available
 * Throws if initial pool is empty
 * Calls OnAcquire before returning acquired item to caller
 * Calls OnRelease before returning item to pool
 */


}

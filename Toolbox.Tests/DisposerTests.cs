using FluentAssertions;
using Xunit;

namespace Toolbox.Tests;

public class DisposerTests
{
    [Fact]
    public void Invokes_action_when_disposed()
    {
        int callCount = 0;
        void action()
        {
            callCount++;
        }

        using (var disposer = new Disposer(action))
        {
            Assert.True(true);
        }
        
        callCount.Should().Be(1);
    }
}
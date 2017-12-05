using System;
using NUnit.Framework;

namespace Toolbox.UnitTests
{
    [TestFixture]
    public class DisposerTests
    {
        [Test]
        public void CallsActionOnDispose()
        {
            bool called = false;
            Action action = () => called = true;
            var disposer = new Disposer(action);

            disposer.Dispose();

            Assert.That(called);
        }
    }
}

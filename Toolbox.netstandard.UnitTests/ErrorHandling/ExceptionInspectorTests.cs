using System;
using NUnit.Framework;
using Toolbox.ErrorHandling;

namespace Toolbox.UnitTests.ErrorHandling
{
    [TestFixture]
    public class ExceptionInspectorTests
    {
        [TestFixture]
        public class GetAllMessages
        {
            private readonly Exception TestEx = new InvalidOperationException("Alpha.", 
                new AppException("Bravo.", new Exception("Charlie."))
                    .SetData("Id", 100)
                    .SetData("Name", "bravo"));

            [Test]
            public void GetsSingleLineMessage()
            {
                var actual = ExceptionInspector.Default.GetAllMessages(TestEx);
                Console.WriteLine(actual);

                Assert.That(actual, Is.EqualTo("Alpha. Bravo. Charlie."));
            }

            [Test]
            public void GetsMultilineMessage()
            {
                var actual = ExceptionInspector.Default.GetAllMessages(TestEx, ExceptionFormatting.Multiline);
                Console.WriteLine(actual);

                Assert.That(actual, Is.EqualTo(@"(InvalidOperation) Alpha.
(App) Bravo.
(Exception) Charlie."));
            }

            [Test]
            public void GetsDetailedMessage()
            {
                var actual = ExceptionInspector.Default.GetAllMessages(TestEx, ExceptionFormatting.Detailed);
                Console.WriteLine(actual);

                Assert.That(actual, Is.EqualTo(@"(InvalidOperation) Alpha.
(App) Bravo.
- Id: 100
- Name: bravo
(Exception) Charlie."));
            }

            [Test]
            public void HandlesAggregateException()
            {
                var exception = new InvalidOperationException("Alpha.",
                    new AggregateException(new AppException("Bravo.", new Exception("Charlie."))
                        .SetData("Id", 100)
                        .SetData("Name", "bravo"), 
                        new AppException("Delta.").SetData("Id", 200)));

                var actual = ExceptionInspector.Default.GetAllMessages(exception, ExceptionFormatting.Detailed);
                Console.WriteLine(actual);

                Assert.That(actual, Is.EqualTo(@"(InvalidOperation) Alpha.
2 errors occurred in parallel:
 1| (App) Bravo.
    - Id: 100
    - Name: bravo
    (Exception) Charlie.
 2| (App) Delta.
    - Id: 200"));
            }

            [Test]
            public void HandlesNestedAggregateExceptions()
            {
                var exception = new AppException("Alpha.",
                    new AggregateException(
                        new AppException("Bravo.", new Exception("Charlie.")),
                        new AggregateException(
                            new AppException("Delta.").SetData("Id", 200),
                            new Exception("Echo.")),
                        new Exception("Foxtrot.")));

                var actual = ExceptionInspector.Default.GetAllMessages(exception, ExceptionFormatting.Multiline);
                Console.WriteLine(actual);

                Assert.That(actual, Is.EqualTo(@"(App) Alpha.
3 errors occurred in parallel:
 1| (App) Bravo.
    (Exception) Charlie.
 2| 2 errors occurred in parallel:
     1| (App) Delta.
     2| (Exception) Echo.
 3| (Exception) Foxtrot."));
            }
        }
    }
}

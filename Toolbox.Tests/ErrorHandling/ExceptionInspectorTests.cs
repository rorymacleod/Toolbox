using System;
using System.Text;
using FluentAssertions;
using Toolbox.ErrorHandling;
using Xunit;

namespace Toolbox.Tests.ErrorHandling;

public class ExceptionInspectorTests
{
    public class GetAllMessages
    {
        private readonly Exception TestEx = new InvalidOperationException("Alpha.",
            new AppException("Bravo.", new Exception("Charlie."))
                .SetData("Id", 100)
                .SetData("Name", "bravo"));

        [Fact]
        public void Gets_single_line_message()
        {
            string message = TestEx.GetAllMessages();
            message.Should().Be("Alpha. Bravo. Charlie.");
        }

        [Fact]
        public void Gets_multi_line_message()
        {
            var expected = new StringBuilder();
            expected.AppendLine("(InvalidOperation) Alpha.");
            expected.AppendLine("(App) Bravo.");
            expected.Append("(Exception) Charlie.");

            var message = TestEx.GetAllMessages(ExceptionFormatting.Multiline);
            message.Should().Be(expected.ToString());
        }

        [Fact]
        public void Gets_detailed_message()
        {
            var expected = new StringBuilder();
            expected.AppendLine("(InvalidOperation) Alpha.");
            expected.AppendLine("(App) Bravo.");
            expected.AppendLine("- Id: 100");
            expected.AppendLine("- Name: bravo");
            expected.Append("(Exception) Charlie.");

            var message = TestEx.GetAllMessages(ExceptionFormatting.Detailed);
            message.Should().Be(expected.ToString());
        }

        [Fact]
        public void Gets_message_with_aggregate_exception()
        {
            var expected = new StringBuilder();
            expected.AppendLine("(InvalidOperation) Alpha.");
            expected.AppendLine("2 errors occurred in parallel:");
            expected.AppendLine(" 1| (App) Bravo.");
            expected.AppendLine("    - Id: 100");
            expected.AppendLine("    - Name: bravo");
            expected.AppendLine("    (Exception) Charlie.");
            expected.AppendLine(" 2| (App) Delta.");
            expected.Append("    - Id: 200");

            var exception = new InvalidOperationException("Alpha.",
                new AggregateException(new AppException("Bravo.", new Exception("Charlie."))
                    .SetData("Id", 100)
                    .SetData("Name", "bravo"),
                    new AppException("Delta.").SetData("Id", 200)));
            var message = exception.GetAllMessages(ExceptionFormatting.Detailed);
            message.Should().Be(expected.ToString());
        }

        [Fact]
        public void Handles_nested_aggregate_exceptions()
        {
            var expected = new StringBuilder();
            expected.AppendLine("(App) Alpha.");
            expected.AppendLine("3 errors occurred in parallel:");
            expected.AppendLine(" 1| (App) Bravo.");
            expected.AppendLine("    (Exception) Charlie.");
            expected.AppendLine(" 2| 2 errors occurred in parallel:");
            expected.AppendLine("     1| (App) Delta.");
            expected.AppendLine("        - Id: 200");
            expected.AppendLine("     2| (Exception) Echo.");
            expected.Append(" 3| (Exception) Foxtrot.");

            var exception = new AppException("Alpha.",
                new AggregateException(
                    new AppException("Bravo.", new Exception("Charlie.")),
                    new AggregateException(
                        new AppException("Delta.").SetData("Id", 200),
                        new Exception("Echo.")),
                    new Exception("Foxtrot.")));

            var message = exception.GetAllMessages(ExceptionFormatting.Detailed);
            message.Should().Be(expected.ToString());
        }
    }
}
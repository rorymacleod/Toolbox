using System;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Toolbox.ErrorHandling;
using Xunit;

namespace Toolbox.Tests.ErrorHandling;

public class ExceptionInspectionTests
{
    private class TestException : Exception
    {
        public TestException() { }
        public TestException(string message) : base(message) { }
        public TestException(string message, Exception innerException) : base(message, innerException) { }
    }

    private static Exception CreateException()
    {
        return new InvalidOperationException("Alpha.",
            new TestException("Bravo.", new Exception("Charlie."))
                .SetData("Id", 100)
                .SetData("Name", "bravo"));
    }

    [Fact]
    public void Get_single_line_message_from_nested_exceptions()
    {
        string message = CreateException().GetAllMessages();
        message.Should().Be("Alpha. Bravo. Charlie.");
    }

    [Fact]
    public void Get_single_line_message_with_data()
    {
        string message = CreateException().GetAllMessages(includeData: true);
        message.Should().Be("Alpha. Bravo (Id=[100], Name=[bravo]). Charlie.");
    }

    [Fact]
    public void Renders_null_values_in_data()
    {
        var exception = new TestException("Alpha").SetData("Id", null);
        string message = exception.GetAllMessages(includeData: true);
        message.Should().Be("Alpha (Id=[<null>]).");
    }

    [Fact]
    public void Get_multiline_message_from_nested_exceptions()
    {
        var expected = new StringBuilder();
        expected.AppendLine("(InvalidOperation) Alpha.");
        expected.AppendLine("(Test) Bravo.");
        expected.Append("(Exception) Charlie.");

        var message = CreateException().GetAllMessages(true);
        message.Should().Be(expected.ToString());
    }

    [Fact]
    public void Get_multiline_message_with_data()
    {
        var expected = new StringBuilder();
        expected.AppendLine("(InvalidOperation) Alpha.");
        expected.AppendLine("(Test) Bravo.");
        expected.AppendLine("- Id=[100]");
        expected.AppendLine("- Name=[bravo]");
        expected.Append("(Exception) Charlie.");

        var message = CreateException().GetAllMessages(true, true);
        message.Should().Be(expected.ToString());
    }

    [Fact]
    public void Gets_multiline_message_with_aggregate_exception()
    {
        var expected = new StringBuilder();
        expected.AppendLine("(InvalidOperation) Alpha.");
        expected.AppendLine("2 errors occurred in parallel:");
        expected.AppendLine(" 1| (Test) Bravo.");
        expected.AppendLine("    (Exception) Charlie.");
        expected.Append(" 2| (Test) Delta.");

        var exception = new InvalidOperationException("Alpha.",
            new AggregateException(
                new TestException("Bravo.", new Exception("Charlie.")),
                new TestException("Delta.")
            ));
        var message = exception.GetAllMessages(true, true);
        message.Should().Be(expected.ToString());
    }

    [Fact]
    public void Gets_multiline_message_with_aggregate_exception_and_data()
    {
        var expected = new StringBuilder();
        expected.AppendLine("(InvalidOperation) Alpha.");
        expected.AppendLine("2 errors occurred in parallel:");
        expected.AppendLine(" 1| (Test) Bravo.");
        expected.AppendLine("    - Id=[100]");
        expected.AppendLine("    - Name=[bravo]");
        expected.AppendLine("    (Exception) Charlie.");
        expected.AppendLine(" 2| (Test) Delta.");
        expected.Append("    - Id=[200]");

        var exception = new InvalidOperationException("Alpha.",
            new AggregateException(
                new TestException("Bravo.", new Exception("Charlie."))
                    .SetData("Id", 100)
                    .SetData("Name", "bravo"),
                new TestException("Delta.").SetData("Id", 200)
            ));
        var message = exception.GetAllMessages(true, true);
        message.Should().Be(expected.ToString());
    }

    [Fact]
    public void Handles_nested_aggregate_exceptions()
    {
        var expected = new StringBuilder();
        expected.AppendLine("(Test) Alpha.");
        expected.AppendLine("3 errors occurred in parallel:");
        expected.AppendLine(" 1| (Test) Bravo.");
        expected.AppendLine("    (Exception) Charlie.");
        expected.AppendLine(" 2| 2 errors occurred in parallel:");
        expected.AppendLine("     1| (Test) Delta.");
        expected.AppendLine("        - Id=[200]");
        expected.AppendLine("     2| (Exception) Echo.");
        expected.Append(" 3| (Exception) Foxtrot.");

        var exception = new TestException("Alpha.",
            new AggregateException(
                new TestException("Bravo.", new Exception("Charlie.")),
                new AggregateException(
                    new TestException("Delta.").SetData("Id", 200),
                    new Exception("Echo.")),
                new Exception("Foxtrot.")));

        var message = exception.GetAllMessages(true, true);
        message.Should().Be(expected.ToString());
    }

    [Fact]
    public void Hides_insignificant_exceptions()
    {
        var exception = new TestException("Alpha.",
            new TargetInvocationException("Bravo.",
                new Exception("Charlie.")));

        var message = exception.GetAllMessages();
        message.Should().Be("Alpha. Charlie.");
    }

    [Fact]
    public void Hides_aggregate_exception_with_one_exception()
    {
        var exception = new AggregateException("Alpha.", new TestException("Bravo."));

        var message = exception.GetAllMessages();
        message.Should().Be("Bravo.");
    }

    [Fact]
    public void Gets_message_when_aggregate_exception_has_zero_exceptions()
    {
        var exception = new AggregateException("Alpha.");

        var message = exception.GetAllMessages();
        message.Should().Be("Alpha.");
    }
}
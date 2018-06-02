using System;
using System.Linq;
using NUnit.Framework;

namespace Toolbox.UnitTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestFixture]
        public class ToHash
        {
            [TestCase("Alpha", "58061aa544398a798e33181a443b15b7746fab16")]
            [TestCase("Alpha1", "2afd643c8465abbf311988fc141204ec1e462c49")]
            [TestCase("Bravo", "f8b83dde53d52771dda7582ed1b5f5928dec9b2f")]
            [TestCase("", "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
            public void ReturnsSHA1Hash(string str, string expected)
            {
                Assert.That(str.ToHash(), Is.EqualTo(expected));
            }

        }

        [TestFixture]
        public class Truncate
        {
            [TestCase("alpha", 10, "alpha")]
            [TestCase("bravo", 5, "bravo")]
            [TestCase("charlie", 4, "char")]
            [TestCase("delta", 0, "")]
            public void ReturnsShortenedString(string str, int length, string expected)
            {
                Assert.That(str.Truncate(length), Is.EqualTo(expected));
            }

            [Test]
            public void ThrowsIfLengthIsLessThanZero()
            {
                try
                {
                    "Alpha".Truncate(-1);
                }
                catch (ArgumentException ex)
                {
                    Assert.That(ex, Is.InstanceOf<ArgumentException>());
                    Assert.That(ex.ParamName, Is.EqualTo("length"));
                }
            }

            [TestCase("Alpha", 10, '…', "Alpha")]
            [TestCase("Bravo", 5, 'x', "Bravo")]
            [TestCase("Charlie", 4, '_', "Cha_")]
            [TestCase("Delta", 1, 'X', "X")]
            public void ReturnsShortenedStringWithCharSuffix(string str, int length, char suffix, string expected)
            {
                Assert.That(str.Truncate(length, suffix), Is.EqualTo(expected));
            }

            [Test]
            public void ThrowsIfLengthWithSuffixIsLessThanOne()
            {
                try
                {
                    "Alpha".Truncate(0, '-');
                }
                catch (ArgumentException ex)
                {
                    Assert.That(ex, Is.InstanceOf<ArgumentException>());
                    Assert.That(ex.ParamName, Is.EqualTo("length"));
                }
            }

            [TestCase("Alpha", 10, "XXX", "Alpha")]
            [TestCase("Bravo", 5, "Y", "Bravo")]
            [TestCase("Charlie", 4, "...", "C...")]
            [TestCase("Delta", 2, "12", "12")]
            public void ReturnsShortenedStringWithStringTail(string str, int length, string tail, string expected)
            {
                Assert.That(str.Truncate(length, tail), Is.EqualTo(expected));
            }

            [Test]
            public void ThrowsIfLengthIsLessThanTail()
            {
                try
                {
                    "Alpha".Truncate(2, "...");
                }
                catch (ArgumentException ex)
                {
                    Assert.That(ex, Is.InstanceOf<ArgumentException>());
                    Assert.That(ex.ParamName, Is.EqualTo("length"));
                }
            }

        }
        
                public class WordWrap
        {
            [Test]
            public void ReturnsEmptyListForEmptyString()
            {
                var actual = string.Empty.WrapLines(10);

                Assert.That(actual.Count(), Is.EqualTo(0));
            }

            [TestCase("A", new[] { "A" })]
            [TestCase("AlphaBravo", new[] { "AlphaBravo" })]
            public void ReturnsSingleLineIfWithinLineLength(string source, string[] expected)
            {
                var actual = source.WrapLines(10);
                Assert.That(actual, Is.EqualTo(expected));
            }

            [TestCase("abc def ghi jkl", new[] { "abc def", "ghi jkl" })]
            [TestCase("abc       def ghi", new[] { "abc", "def ghi" })]
            [TestCase("AlphaBravo Zulu", new[] { "AlphaBravo", "Zulu" })]
            public void ReturnsLinesWrappedOnSpaces(string source, string[] expected)
            {
                var actual = source.WrapLines(10);
                Assert.That(actual, Is.EqualTo(expected));
            }

            [TestCase("abc def__ghi jkl", new[] { "abc", "def__ghi", "jkl" })]
            [TestCase("abc def\t\tghi jkl", new[] { "abc def", "ghi jkl" })]
            public void ReturnsLinesWrappedOnTabs(string source, string[] expected)
            {
                var actual = source.WrapLines(10);
                Assert.That(actual, Is.EqualTo(expected));
            }

            [TestCase("abc def__ghi jkl", new[] { "abc", "def__ghi", "jkl" })]
            [TestCase("abc def\t\tghi jkl", new[] { "abc def", "ghi jkl" })]
            public void PreservesNewLines()
            {
                Assert.Ignore("The test has not been implemented.");
            }

            [Test]
            public void PreservesWhitespaceAfterNewLines()
            {
                Assert.Ignore("The test has not been implemented.");
            }
        }

    }
}

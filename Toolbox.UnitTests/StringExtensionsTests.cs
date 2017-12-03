using System;
using NUnit.Framework;

namespace Toolbox.UnitTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
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
    }
}

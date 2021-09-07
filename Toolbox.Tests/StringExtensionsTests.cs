using FluentAssertions;
using System;
using Xunit;

namespace Toolbox.Tests;

public class StringExtensionsTests
{
    public class ToHash
    {
        [Theory]
        [InlineData("Alpha", "58061aa544398a798e33181a443b15b7746fab16")]
        [InlineData("Alpha1", "2afd643c8465abbf311988fc141204ec1e462c49")]
        [InlineData("Bravo", "f8b83dde53d52771dda7582ed1b5f5928dec9b2f")]
        [InlineData("", "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        public void Returns_SHA1_hash(string str, string expected)
        {
            string hash = str.ToHash();

            hash.Should().Be(expected);
        }
    }

    public class Truncate
    {
        [Theory]
        [InlineData("alpha", 10, "alpha")]
        [InlineData("bravo", 5, "bravo")]
        [InlineData("charlie", 4, "char")]
        [InlineData("delta", 0, "")]
        public void Returns_shortened_string(string str, int length, string expected)
        {
            str.Truncate(length).Should().Be(expected);
        }

        [Fact]
        public void Throws_if_length_is_less_than_zero()
        {
            try
            {
                "Alpha".Truncate(-1);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be("length");
                return;
            }
                
            Assert.True(false, "Expected exception was not thrown.");
        }

        [Theory]
        [InlineData("Alpha", 10, '…', "Alpha")]
        [InlineData("Bravo", 5, 'x', "Bravo")]
        [InlineData("Charlie", 4, '_', "Cha_")]
        [InlineData("Delta", 1, 'X', "X")]
        public void Returns_shortened_string_with_char_suffix(string str, int length, char suffix, string expected)
        {
            str.Truncate(length, suffix).Should().Be(expected);
        }

        [Fact]
        public void Throws_if_length_with_suffix_is_less_than_one()
        {
            try
            {
                "Alpha".Truncate(0, '-');
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be("length");
                return;
            }
                
            Assert.True(false, "Exception was not thrown");
        }

        [Theory]
        [InlineData("Alpha", 10, "XXX", "Alpha")]
        [InlineData("Bravo", 5, "Y", "Bravo")]
        [InlineData("Charlie", 4, "...", "C...")]
        [InlineData("Delta", 2, "12", "12")]
        public void Returns_shortened_string_with_string_tail(string str, int length, string tail, string expected)
        {
            str.Truncate(length, tail).Should().Be(expected);
        }

        [Fact]
        public void Throws_if_length_is_less_than_tail()
        {
            try
            {
                "Alpha".Truncate(2, "...");
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be("length");
                return;
            }
                
            Assert.True(false, "Expected exception was not thrown.");
        }
    }
}

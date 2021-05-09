using System;
using BenchmarkDotNetAnalyser.Instrumentation;
using FluentAssertions;
using FsCheck.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Instrumentation
{
    public class ColourExtensionsTests
    {
        [Fact]
        public void Colourise_NullValue_ReturnsNull()
        {
            string value = null;

            var r = value.Colourise(ConsoleColor.Blue);

            r.Should().BeNullOrEmpty();
        }

        [Property(Verbose = true, Arbitrary = new[] {typeof(AlphanumericStringArbitrary)})]
        public bool Colourise_ReturnsAnnotatedValue(string value, ConsoleColor colour)
        {
            var result = value.Colourise(colour);
            
            return result.Contains(value) && result.Length > value.Length;
        }


    }
}

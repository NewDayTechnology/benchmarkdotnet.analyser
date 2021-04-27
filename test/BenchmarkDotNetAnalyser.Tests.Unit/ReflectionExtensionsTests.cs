using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public class ReflectionExtensionsTests
    {
        private class StubAttribute : Attribute
        {
        }

        [Fact]
        public void GetAttributeValue_CopyrightValueRetrieved()
        {
            var attrs = typeof(Program).Assembly.GetCustomAttributes();

            var r = attrs.GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright);

            r.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetAttributeValue_AttributeNotRetrieved()
        {
            var attrs = typeof(Program).Assembly.GetCustomAttributes();

            var r = attrs.GetAttributeValue<StubAttribute>(a => a.ToString());

            r.Should().BeNullOrEmpty();
        }
    }
}

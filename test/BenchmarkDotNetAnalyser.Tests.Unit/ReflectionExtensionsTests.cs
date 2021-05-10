using System;
using System.ComponentModel;
using System.Linq;
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

        private class StubClass
        {
            [Stub] public string Member1 { get; }
            [Stub, Description] public string Member2 { get; }
            [Description] public string Member3 { get; }
        }

        private class StubSubClass : StubClass { }

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

        [Fact]
        public void GetMemberAttributePairs_UnknownAttribute_ReturnsEmpty()
        {
            var r = typeof(Program).GetMemberAttributePairs<StubAttribute>().ToList();
            r.Should().BeEmpty();
        }

        [Fact]
        public void GetMemberAttributePairs_UsedAttribute_ReturnsNonEmpty()
        {
            var r = typeof(StubClass).GetMemberAttributePairs<StubAttribute>().ToList();
            
            var expectedMembers = new[] {nameof(StubClass.Member1), nameof(StubClass.Member2)};
            var actualMembers = r.Select(t => t.Item1.Name);
            var actualAttributes = r.Select(t => t.Item2);

            actualMembers.Should().BeEquivalentTo(expectedMembers);
            actualAttributes.All(a => a is StubAttribute).Should().BeTrue();
        }

        [Fact]
        public void GetMemberAttributePairs_UsedAttributeInBaseClass_ReturnsNonEmpty()
        {
            var r = typeof(StubSubClass).GetMemberAttributePairs<StubAttribute>().ToList();
            var expectedMembers = new[] {nameof(StubClass.Member1), nameof(StubClass.Member2)};
            var actualMembers = r.Select(t => t.Item1.Name);
            var actualAttributes = r.Select(t => t.Item2);

            actualMembers.Should().BeEquivalentTo(expectedMembers);
            actualAttributes.All(a => a is StubAttribute).Should().BeTrue();
        }

    }
}

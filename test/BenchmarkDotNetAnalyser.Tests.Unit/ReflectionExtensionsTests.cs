using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Shouldly;
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

            r.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetAttributeValue_AttributeNotRetrieved()
        {
            var attrs = typeof(Program).Assembly.GetCustomAttributes();

            var r = attrs.GetAttributeValue<StubAttribute>(a => a.ToString());

            r.ShouldBeNullOrEmpty();
        }

        [Fact]
        public void GetMemberAttributePairs_UnknownAttribute_ReturnsEmpty()
        {
            var r = typeof(Program).GetMemberAttributePairs<StubAttribute>().ToList();
            r.ShouldBeEmpty();
        }

        [Fact]
        public void GetMemberAttributePairs_UsedAttribute_ReturnsNonEmpty()
        {
            var r = typeof(StubClass).GetMemberAttributePairs<StubAttribute>().ToList();
            
            var expectedMembers = new[] {nameof(StubClass.Member1), nameof(StubClass.Member2)};
            var actualMembers = r.Select(t => t.Item1.Name);
            var actualAttributes = r.Select(t => t.Item2);

            actualMembers.ShouldBe(expectedMembers);
            actualAttributes.All(a => a is StubAttribute).ShouldBeTrue();
        }

        [Fact]
        public void GetMemberAttributePairs_UsedAttributeInBaseClass_ReturnsNonEmpty()
        {
            var r = typeof(StubSubClass).GetMemberAttributePairs<StubAttribute>().ToList();
            var expectedMembers = new[] {nameof(StubClass.Member1), nameof(StubClass.Member2)};
            var actualMembers = r.Select(t => t.Item1.Name);
            var actualAttributes = r.Select(t => t.Item2);

            actualMembers.ShouldBe(expectedMembers);
            actualAttributes.All(a => a is StubAttribute).ShouldBeTrue();
        }

    }
}

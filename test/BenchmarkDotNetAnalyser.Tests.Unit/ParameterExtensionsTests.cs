using System;
using Shouldly;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public class ParameterExtensionsTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        public void ArgNotNull_NoExceptionThrown(string value)
        {
            value.ArgNotNull("paramName");
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        public void ArgNotNull_ExceptionThrown(string paramName)
        {
            try
            {
                string value = null;

                value.ArgNotNull(paramName);
            }
            catch (ArgumentNullException ex)
            {
                ex.ParamName.ShouldBe(paramName);
            }
        }

        [Property]
        public bool InvalidOpArg_PredicatePasses_ValueReturned(NonEmptyString value)
        {
            var r = value.Get.InvalidOpArg(s => s.Length == 0, "message");

            return r == value.Get;
        }

        [Property]
        public bool InvalidOpArg_PredicateTrue_ExceptionThrown(NonEmptyString message)
        {
            try
            {
                var r = "abc".InvalidOpArg(s => true, message.Get);

            }
            catch (InvalidOperationException ex)
            {
                return ex.Message == message.Get;
            }

            return false;
        }

        [Fact]
        public void InvalidOpArg_PredicateNull_ExceptionThrown()
        {
            Func<string, bool> sel = null;

            Action f = () => "abc".InvalidOpArg(sel, "message");

            f.ShouldThrow<ArgumentNullException>();
        }
    }
}

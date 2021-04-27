using System;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public class ObjectExtensionsTests
    {
        [Property]
        public bool Pipe_ValuePiped(PositiveInt value)
        {
            Func<int, string> f = x => x.ToString();

            var r = value.Get.Pipe(f);

            return r == f(value.Get);
        }

        [Fact]
        public void Pipe_NullSelector_ExceptionThrown()
        {
            Func<string, int> s = null;
            Func<int> f = () => "".Pipe(s);

            f.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Pipe_NullValue_ExceptionThrown()
        {
            string str = null;
            Func<string, int> sel = s => { throw new AccessViolationException(); };
            
            Func<int> f = () => str.Pipe(sel); // for clarity throw a completely unrelated exception 

            f.Should().Throw<AccessViolationException>();
        }

        [Property]
        public bool PipeIfNotNull_ValuePiped(NonEmptyString value)
        {
            Func<string, int> f = x => x.Length;

            var r = value.Get.PipeIfNotNull(f);

            return r == f(value.Get);
        }

        
        [Fact]
        public void PipeIfNotNull_NullSelector_ExceptionThrown()
        {
            Func<string, int> s = null;
            Func<int> f = () => "".PipeIfNotNull(s);

            f.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PipeIfNotNull_NullValue_NullNotPropagated()
        {
            string str = null;

            int defaultValue = 1234;
            var r = str.PipeIfNotNull(s => s.Length, defaultValue);

            r.Should().Be(defaultValue);
        }

        [Property]
        public bool PipeDo_ValuePiped(PositiveInt value)
        {
            var r = 0;

            Func<int, int> f = x => x * 2; // double the value to ensure function invoked
            value.Get.PipeDo(x => r = f(x));

            return r == f(value.Get);
        }
    }
}

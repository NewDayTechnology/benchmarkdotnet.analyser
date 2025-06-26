using System;
using Shouldly;
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
            Action f = () => "".Pipe(s);

            f.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Pipe_NullValue_ExceptionThrown()
        {
            string str = null;
            Func<string, int> sel = s => { throw new AccessViolationException(); };
            
            Action f = () => str.Pipe(sel); // for clarity throw a completely unrelated exception 

            f.ShouldThrow<AccessViolationException>();
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
            Action f = () => "".PipeIfNotNull(s);

            f.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PipeIfNotNull_NullValue_NullNotPropagated()
        {
            string str = null;

            int defaultValue = 1234;
            var r = str.PipeIfNotNull(s => s.Length, defaultValue);

            r.ShouldBe(defaultValue);
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

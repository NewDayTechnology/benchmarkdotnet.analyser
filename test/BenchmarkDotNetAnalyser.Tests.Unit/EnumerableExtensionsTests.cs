using System;
using System.Linq;
using Shouldly;

using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void MinBy_Decimal_EmptySequence_ExceptionThrown()
        {
            var xs = Enumerable.Empty<decimal>();

            Action f = () => xs.MinBy(x => x);

            f.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void MinBy_Decimal_EmptySequence_ExceptionMessageNonEmpty()
        {
            var xs = Enumerable.Empty<decimal>();

            try
            {
                var r = xs.MinBy(x => x);
            }
            catch (InvalidOperationException e)
            {
                if (string.IsNullOrWhiteSpace(e.Message))
                {
                    throw new Exception("Missing message");
                }
            }
        }

        [Property(Verbose = true)]
        public bool MinBy_Decimal_FindsMinByReverse(PositiveInt count)
        {
            var xs = Enumerable.Range(1, count.Get).Select(x => (decimal)x).ToArray();
            
            var r1 = xs.MinBy(x => x);
            var r2 = xs.Min();

            return r1 == r2;
        }

        [Property(Verbose = true)]
        public bool MinBy_Decimal_Reversed_FindsMinByReverse(PositiveInt count)
        {
            var xs = Enumerable.Range(1, count.Get).Select(x => (decimal)x).ToArray();
            
            var r1 = xs.Reverse().MinBy(x => x);
            var r2 = xs.Min();

            return r1 == r2;
        }

        [Property(Verbose = true)]
        public bool MinBy_Decimal_ValuesEqual_FindsMinByReverse(PositiveInt count)
        {
            var xs = Enumerable.Range(1, count.Get).Select(x => (decimal)x).ToList();
            var r2 = xs.Min();
            xs.Add(r2);

            var r1 = xs.MinBy(x => x);
            
            return r1 == r2;
        }

        [Property(Verbose = true)]
        public bool MinBy_Decimal_StringValueProjections_FindsMinByReverse(PositiveInt count)
        {
            var xs = Enumerable.Range(1, count.Get).Select(x => x.ToString()).ToArray();

            var r1 = xs.Reverse().MinBy(decimal.Parse);
            var r2 = xs.Min();

            return r1 == r2;
        }

        [Property(Verbose = true)]
        public bool ToInfinity_ValuesRepeated(PositiveInt count)
        {
            var xs = Enumerable.Range(1, count.Get);

            var ys = xs.ToInfinity();

            int repeats = 3;
            var zs = ys.Take(count.Get * repeats);

            var expected = xs.Concat(xs).Concat(xs);

            return zs.SequenceEqual(expected);
        }
    }
}

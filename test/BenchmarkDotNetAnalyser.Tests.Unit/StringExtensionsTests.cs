using System;
using System.Globalization;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public class StringExtensionsTests
    {
        [Property(Verbose = true)]
        public bool Join_DelimitersInterspersed(PositiveInt segments)
        {
            var delimiter = ",";
            var d = delimiter.ToCharArray();
            var xs = Enumerable.Range(1, segments.Get).Select(_ => "A");
            var r = xs.Join(delimiter);

            var delimiterCount = r.Count(c => c == d[0]);

            return delimiterCount == (segments.Get - 1);
        }

        [Property(Verbose = true)]
        public bool Join_SegmentsPreserved(PositiveInt segments)
        {
            var s = 'A';
            var xs = Enumerable.Range(1, segments.Get).Select(_ => s.ToString());
            var r = xs.Join(",");

            var segmentCount = r.Count(c => c == s);

            return segmentCount == segments.Get;
        }

        [Property(Verbose = true)]
        public bool Join_SegmentsInterspersed(PositiveInt segments)
        {
            var s = 'A';
            var delimiter = ",,";
            var xs = Enumerable.Range(1, segments.Get).Select(_ => s.ToString());
            var r = xs.Join(delimiter);

            var indices = r.Select((c, i) => (c, i))
                .Where(t => t.c == s)
                .Select(t => t.i);

            
            var expected = Enumerable.Range(0, segments.Get)
                .Select(x => x * (delimiter.Length + 1));

            return indices.SequenceEqual(expected);
        }

        [Property(Verbose = true)]
        public bool ToPercentageDecimal_Passes(PositiveInt value)
        {
            var decValue = ((decimal) value.Get);
            decValue += (decValue / 100);
            var expected = decValue / 100;

            var s = decValue.ToString(CultureInfo.InvariantCulture);

            var r = s.ToPercentageDecimal();

            return r == expected;
        }



        [Property(Verbose = true)]
        public bool ToPercentageDecimal_InvalidNumber_ExceptionThrown(NonEmptyString value)
        {
            try
            {
                var r = (value.Get + "A").ToPercentageDecimal();
                return false;
            }
            catch (FormatException)
            {
                return true;
            }
        }

        
        [Property(Verbose = true)]
        public bool ToDecimal_InvalidNumber_ExceptionThrown(NonEmptyString value)
        {
            try
            {
                var s = value.Get + "A";
                var r = s.ToDecimal();
                return false;
            }
            catch (FormatException)
            {
                return true;
            }
        }

        [Property(Verbose = true)]
        public bool ToDecimal_Passes(PositiveInt value)
        {
            var decValue = ((decimal) value.Get);
            decValue += (decValue / 100);
            
            var s = decValue.ToString(CultureInfo.InvariantCulture);

            var r = s.ToDecimal();

            return r == decValue;
        }
        
        [Property(Verbose = true)]
        public bool ToInt_InvalidNumber_ExceptionThrown(NonEmptyString value)
        {
            try
            {
                var r = (value.Get + "A").ToInt();
                return false;
            }
            catch (FormatException)
            {
                return true;
            }
        }

        [Property(Verbose = true)]
        public bool ToInt_Passes(PositiveInt value)
        {
            var decValue = ((decimal) value.Get);
            
            var s = decValue.ToString(CultureInfo.InvariantCulture);

            var r = s.ToInt();

            return r == decValue;
        }
    }
}

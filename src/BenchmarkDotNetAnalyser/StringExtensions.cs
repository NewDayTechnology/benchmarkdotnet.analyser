using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser
{
    internal static class StringExtensions
    {
        public static string Join(this IEnumerable<string> values, string delimiter) =>
                                    String.Join(delimiter, values);

        public static string Format(this string value, string format) => string.Format(format, value);

        public static int ToInt(this string value) => Int32.Parse(value);
        public static decimal ToDecimal(this string value) => Decimal.Parse(value);

        public static decimal ToPercentageDecimal(this string value) => Decimal.Parse(value) / 100;

        public static bool IsMatch(this string value, string pattern)
        {
            if (value == null) return false;
            if (pattern == null) return true;
            if (pattern == "") return false;
            if (StringComparer.OrdinalIgnoreCase.Equals(pattern, "*")) return true;

            var end = pattern.Length - 1;
            var v = value.AsSpan();
            var p = pattern.AsSpan();
            
            if(p[0] == '*')
            {
                if(p[end] == '*')
                {
                    return v.Contains(p.Slice(1, end - 1), StringComparison.OrdinalIgnoreCase);
                }

                return v.EndsWith(p.Slice(1), StringComparison.OrdinalIgnoreCase);
            }
            if(p[end] == '*')
            {
                return v.StartsWith(p.Slice(0, end - 1), StringComparison.OrdinalIgnoreCase);
            }

            return v.Equals(p, StringComparison.OrdinalIgnoreCase);
        }
    }
}

using System;
using System.Collections.Generic;

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
    }
}

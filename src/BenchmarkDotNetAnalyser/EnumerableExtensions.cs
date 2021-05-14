using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BenchmarkDotNetAnalyser
{
    internal static class EnumerableExtensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<T> Singleton<T>(this T value)
        {
            yield return value;
        }

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
        {
            return values == null || !values.Any();
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> values) => values ?? Enumerable.Empty<T>();

        [DebuggerStepThrough]
        public static T MinBy<T>(this IEnumerable<T> values, Func<T, decimal> selector)
        {
            values.ArgNotNull(nameof(values));
            selector.ArgNotNull(nameof(selector));

            var best = decimal.MaxValue;
            T result = default;
            var found = false;

            foreach (var value in values)
            {
                found = true;
                var v = selector(value);
                if (v < best)
                {
                    best = v;
                    result = value;
                }
            }

            found.InvalidOpArg(x => !x, "Sequence contains no elements.");

            return result;
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> ToInfinity<T>(this IEnumerable<T> values)
        {
            while (true)
            {
                foreach (var value in values)
                {
                    yield return value;
                }
            }
        }
    }
}

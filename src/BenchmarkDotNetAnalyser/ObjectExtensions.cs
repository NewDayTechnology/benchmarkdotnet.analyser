using System;
using System.Diagnostics;

namespace BenchmarkDotNetAnalyser
{
    internal static class ObjectExtensions
    {
        [DebuggerStepThrough]
        public static TResult Pipe<TValue, TResult>(this TValue value, Func<TValue, TResult> selector)
        {
            selector.ArgNotNull(nameof(selector));

            return selector(value);
        }

        [DebuggerStepThrough]
        public static TResult PipeIfNotNull<TValue, TResult>(this TValue value, Func<TValue, TResult> selector, TResult defaultValue = default)
                where TValue : class
        {
            selector.ArgNotNull(nameof(selector));

            return value != null
                ? selector(value)
                : defaultValue;
        }


        [DebuggerStepThrough]
        public static void PipeDo<TValue>(this TValue value, Action<TValue> selector)
        {
            selector.ArgNotNull(nameof(selector));

            selector(value);
        }

    }
}

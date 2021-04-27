using System;
using System.Diagnostics;

namespace BenchmarkDotNetAnalyser
{
    internal static class ParameterExtensions
    {
        [DebuggerStepThrough]
        public static T ArgNotNull<T>(this T value, string paramName) where T : class
        {
            if(ReferenceEquals(null, value))
            {
                throw new ArgumentNullException(paramName: paramName);
            }
            return value;
        }

        [DebuggerStepThrough]
        public static T InvalidOpArg<T>(this T value, Func<T, bool> predicate, string message)
        {
            predicate.ArgNotNull(nameof(predicate));

            if (ReferenceEquals(null, value)|| predicate(value))
            {
                throw new InvalidOperationException(message);
            }

            return value;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser
{
    internal static class ReflectionExtensions
    {
        public static string GetAttributeValue<T>(this IEnumerable<Attribute> attributes, Func<T, string> selector)
            where T : Attribute
        {
            return attributes.OfType<T>().FirstOrDefault().PipeIfNotNull(selector);
        }
    }
}

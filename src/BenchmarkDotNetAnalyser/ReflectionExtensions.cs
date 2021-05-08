using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BenchmarkDotNetAnalyser
{
    internal static class ReflectionExtensions
    {
        public static string GetAttributeValue<T>(this IEnumerable<Attribute> attributes, Func<T, string> selector)
            where T : Attribute
        {
            return attributes.OfType<T>().FirstOrDefault().PipeIfNotNull(selector);
        }

        public static IEnumerable<(MemberInfo, T)> GetMemberAttributePairs<T>(this Type type)
            where T : Attribute
        {
            return type.GetMembers().Select(mi => (mi, mi.GetCustomAttributes()
                    .OfType<T>()
                    .FirstOrDefault()))
                    .Where(t => t.Item2 != null);
        }
    }
}

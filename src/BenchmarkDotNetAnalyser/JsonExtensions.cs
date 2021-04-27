using Newtonsoft.Json.Linq;

namespace BenchmarkDotNetAnalyser
{
    internal static class JsonExtensions
    {
        public static JToken GetToken(this JObject value, string name) => value[name];
        
        public static decimal GetDecimalValue(this JToken value, string name)
        {
            var jt = value[name];
            if (jt == null) return default;

            return jt.Value<decimal>();
        }

        public static string GetStringValue(this JToken value, string name)
        {
            var jt = value[name];

            return jt?.Value<string>();
        }

        public static int GetIntValue(this JToken value, string name)
        {
            var jt = value[name];
            if (jt == null) return default;

            return jt.Value<int>();
        }
    }
}

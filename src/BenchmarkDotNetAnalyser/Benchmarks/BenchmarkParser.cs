using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public class BenchmarkParser
    {
        private readonly Lazy<JObject> _jsonObject;

        public BenchmarkParser(string json)
        {
            json.ArgNotNull(nameof(json));

            _jsonObject = new Lazy<JObject>(Parse(json));
        }

        public DateTimeOffset GetCreation()
        {
            var s = _jsonObject.Value.GetStringValue("Title");
            if (s == null) return default;

            var i = s.IndexOf('-');
            var v = s.Substring(i + 1);

            return ParseDateTime(v);
        }

        public BenchmarkEnvironment GetBenchmarkEnvironment()
        {
            var env = _jsonObject.Value.GetToken("HostEnvironmentInfo");

            return new BenchmarkEnvironment()
            {
                BenchmarkDotNetVersion = env.GetStringValue("BenchmarkDotNetVersion"),
                DotNetCliVersion = env.GetStringValue("DotNetCliVersion"),
                DotNetRuntimeVersion = env.GetStringValue("RuntimeVersion"),
                PhysicalProcessorCount = env.GetIntValue("PhysicalProcessorCount"),
                LogicalCoreCount = env.GetIntValue("LogicalCoreCount"),
                MachineArchitecture = env.GetStringValue("Architecture"),
                OsVersion = env.GetStringValue("OsVersion"),
                ProcessorName = env.GetStringValue("ProcessorName"),
            };
        }

        public IEnumerable<BenchmarkResult> GetBenchmarkResults()
        {
            BenchmarkResult ParseResult(JObject bm)
            {
                var stat = bm.GetToken("Statistics");
                
                var memory = bm.GetToken("Memory");

                return new BenchmarkResult()
                {
                    FullName = bm.GetStringValue("FullName"),
                    Namespace = bm.GetStringValue("Namespace"),
                    Type = bm.GetStringValue("Type"),
                    Method = bm.GetStringValue("Method"),
                    Parameters = bm.GetStringValue("Parameters"),
                    MaxTime = stat?.GetDecimalValue("Max"),
                    MinTime = stat?.GetDecimalValue("Min"),
                    MeanTime = stat?.GetDecimalValue("Mean"),
                    MedianTime = stat?.GetDecimalValue("Median"),
                    Q1Time = stat?.GetDecimalValue("Q1"),
                    Q3Time = stat?.GetDecimalValue("Q3"),
                    Gen0Collections = memory?.GetDecimalValue("Gen0Collections"),
                    Gen1Collections = memory?.GetDecimalValue("Gen1Collections"),
                    Gen2Collections = memory?.GetDecimalValue("Gen2Collections"),
                    TotalOps = memory?.GetDecimalValue("TotalOperations"),
                    BytesAllocatedPerOp = memory?.GetDecimalValue("BytesAllocatedPerOperation")
                };
            }

            return _jsonObject.Value.GetToken("Benchmarks")
                                .OfType<JObject>().NullToEmpty()
                                .Select(ParseResult)
                                .ToList();
        }

        private JObject Parse(string json) => Newtonsoft.Json.JsonConvert.DeserializeObject(json) as JObject;
        

        private DateTime ParseDateTime(string value) => DateTime.ParseExact(value, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
    }
}

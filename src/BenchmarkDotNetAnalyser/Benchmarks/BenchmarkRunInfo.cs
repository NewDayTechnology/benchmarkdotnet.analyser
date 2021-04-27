using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkRunInfo
    {
        [JsonProperty("creation")]
        public DateTimeOffset Creation { get; set; }
        [JsonProperty("benchmarkDotNetVersion")]
        public string BenchmarkDotNetVersion { get; set; }
        [JsonProperty("fullPath")]
        public string FullPath { get; set; }
    }
}

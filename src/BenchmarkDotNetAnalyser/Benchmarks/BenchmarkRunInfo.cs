using System;
using System.Collections.Generic;
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
        
        [JsonProperty("results")]
        public IList<BenchmarkResult> Results { get; set; }
    }
}

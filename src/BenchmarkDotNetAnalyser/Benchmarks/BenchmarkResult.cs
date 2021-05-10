using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkResult
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("parameters")]
        public string Parameters { get; set; }

        [JsonProperty("meanTime")]
        public decimal? MeanTime { get; set; }
        
        [JsonProperty("minTime")]
        public decimal? MinTime { get; set; }

        [JsonProperty("q1Time")]
        public decimal? Q1Time { get; set; }

        [JsonProperty("medianTime")]
        public decimal? MedianTime { get; set; }

        [JsonProperty("q3Time")]
        public decimal? Q3Time { get; set; }

        [JsonProperty("maxTime")]
        public decimal? MaxTime { get; set; }
    }
}

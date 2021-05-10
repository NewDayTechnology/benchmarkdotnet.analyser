using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkInfo
    {
        [JsonProperty("creation")]
        public DateTimeOffset Creation { get; set; }
        
        [JsonProperty("pinned")]
        public bool Pinned { get; set; }
        
        [JsonProperty("buildUri")]
        public string BuildUri { get; set; }
        
        [JsonProperty("branchName")]
        public string BranchName { get; set; }

        [JsonProperty("tags")]
        public IList<string> Tags { get; set; }

        [JsonProperty("runs")]
        public IList<BenchmarkRunInfo> Runs { get; set; }
    }
}

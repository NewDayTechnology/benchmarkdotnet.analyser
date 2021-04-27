using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Analysis
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkResultGroup
    {
        public string Name { get; set; }
        public IList<(BenchmarkRunInfo, BenchmarkResult)> Results { get; set; }
    }
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkRunResults
    {
        public BenchmarkRunInfo Run { get; set; }
        public IList<BenchmarkResult> Results { get; set; }
    }
}

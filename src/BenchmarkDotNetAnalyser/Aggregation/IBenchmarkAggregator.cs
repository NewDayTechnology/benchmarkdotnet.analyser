using System.Collections.Generic;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Aggregation
{
    public interface IBenchmarkAggregator
    {
        IEnumerable<BenchmarkInfo> Aggregate(BenchmarkAggregationOptions options, IEnumerable<BenchmarkInfo> aggregates,
            BenchmarkInfo newBenchmark);
    }
}

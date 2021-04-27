using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Aggregation
{
    public class BenchmarkAggregator : IBenchmarkAggregator
    {
        public IEnumerable<BenchmarkInfo> Aggregate(BenchmarkAggregationOptions options, IEnumerable<BenchmarkInfo> aggregates, BenchmarkInfo newBenchmark)
        {
            newBenchmark.ArgNotNull(nameof(newBenchmark));
            options.ArgNotNull(nameof(options));
            
            aggregates = TrimAggregates(options, aggregates.NullToEmpty());
            
            return newBenchmark.Singleton().Concat(aggregates);
        }

        private IEnumerable<BenchmarkInfo> TrimAggregates(BenchmarkAggregationOptions options,
            IEnumerable<BenchmarkInfo> aggregates)
        {
            var maxRuns = options.Runs - 1;

            return options.PreservePinned 
                    ? PreservePinnedAggregates(maxRuns, aggregates)
                    : aggregates.Take(maxRuns);
        }

        private IEnumerable<BenchmarkInfo> PreservePinnedAggregates(int maxRuns,
                                                                    IEnumerable<BenchmarkInfo> aggregates)
        {
            var yielded = 0;
            foreach (var info in aggregates)
            {
                if (info.Pinned)
                {
                    yield return info;
                }
                else if (yielded++ < maxRuns)
                {
                    yield return info;
                }
            }
        }
        
    }
}

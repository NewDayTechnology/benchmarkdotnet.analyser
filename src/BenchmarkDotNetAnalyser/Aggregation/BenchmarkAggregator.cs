using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Aggregation
{
    public class BenchmarkAggregator : IBenchmarkAggregator
    {
        private readonly IBenchmarkStatisticAccessorProvider _statisticAccessor;

        public BenchmarkAggregator(IBenchmarkStatisticAccessorProvider statisticAccessor)
        {
            _statisticAccessor = statisticAccessor.ArgNotNull(nameof(statisticAccessor));
        }

        public IEnumerable<BenchmarkInfo> Aggregate(BenchmarkAggregationOptions options, IEnumerable<BenchmarkInfo> aggregates, BenchmarkInfo newBenchmark)
        {
            newBenchmark.ArgNotNull(nameof(newBenchmark));
            options.ArgNotNull(nameof(options));

            aggregates = aggregates.NullToEmpty();

            aggregates = aggregates.PinBest(_statisticAccessor);

            aggregates = TrimAggregates(options, aggregates);
            
            return newBenchmark.Singleton().Concat(aggregates);
        }

        private IEnumerable<BenchmarkInfo> TrimAggregates(BenchmarkAggregationOptions options,
            IEnumerable<BenchmarkInfo> benchmarkInfos)
        {
            var maxRuns = options.Runs - 1;

            return options.PreservePinned 
                    ? benchmarkInfos.PreservePinned(maxRuns)
                    : benchmarkInfos.Take(maxRuns);
        }
    }
}

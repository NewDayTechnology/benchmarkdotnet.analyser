using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class BenchmarkResultAnalysisAggregator
    {
        private readonly int _maxFailures;

        public BenchmarkResultAnalysisAggregator(int maxFailures)
        {
            _maxFailures = maxFailures;
        }

        public BenchmarkResultAnalysis Consolidate(IEnumerable<BenchmarkResultAnalysis> results)
        {
            var innerResults = results.ArgNotNull(nameof(results))
                                      .ToList();
            var failures = innerResults.Count(r => !r.MeetsRequirements);
            
            return new BenchmarkResultAnalysis()
            {
                MeetsRequirements = failures <= _maxFailures,
                InnerResults = innerResults,
            };
        }
    }
}

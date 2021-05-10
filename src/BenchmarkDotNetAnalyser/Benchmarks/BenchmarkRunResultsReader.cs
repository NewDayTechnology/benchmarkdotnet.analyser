using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    internal class BenchmarkRunResultsReader : IBenchmarkRunResultsReader
    {
        public IList<BenchmarkRunResults> GetBenchmarkResults(IEnumerable<BenchmarkInfo> benchmarks) =>
            benchmarks.NullToEmpty()
                .SelectMany(GetBenchmarkResults)
                .ToList();
        
        private IEnumerable<BenchmarkRunResults> GetBenchmarkResults(BenchmarkInfo benchmark) =>
            benchmark.Runs.Select(bri => new BenchmarkRunResults()
            {
                Run = bri,
                Results = bri.Results,
            });
    }
}

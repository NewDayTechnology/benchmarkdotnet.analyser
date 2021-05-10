using System.Collections.Generic;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public interface IBenchmarkRunResultsReader
    {
        IList<BenchmarkRunResults> GetBenchmarkResults(IEnumerable<BenchmarkInfo> benchmarks);
    }
}

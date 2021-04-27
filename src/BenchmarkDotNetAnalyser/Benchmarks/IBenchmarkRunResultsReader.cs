using System.Collections.Generic;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public interface IBenchmarkRunResultsReader
    {
        Task<IList<BenchmarkRunResults>> GetBenchmarkResults(IEnumerable<BenchmarkInfo> benchmarks);
    }
}

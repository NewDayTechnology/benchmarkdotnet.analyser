using System.Collections.Generic;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public interface IBenchmarkResultReader
    {
        Task<IList<BenchmarkResult>> GetBenchmarkResultsAsync(string path);
    }
}

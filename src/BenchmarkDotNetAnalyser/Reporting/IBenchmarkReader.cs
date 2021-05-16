using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Reporting
{
    public interface IBenchmarkReader
    {
        Task<IEnumerable<BenchmarkInfo>> GetBenchmarkAsync(string path, IList<string> filters);
    }
}

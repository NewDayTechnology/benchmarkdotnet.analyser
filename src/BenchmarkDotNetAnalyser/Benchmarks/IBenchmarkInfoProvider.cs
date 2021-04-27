using System.Collections.Generic;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public interface IBenchmarkInfoProvider
    {
        Task<IList<BenchmarkInfo>> GetBenchmarkInfosAsync(string path);
        
        Task WriteBenchmarkInfosAsync(string sourcePath, string destinationPath, IEnumerable<BenchmarkInfo> values);
    }
}

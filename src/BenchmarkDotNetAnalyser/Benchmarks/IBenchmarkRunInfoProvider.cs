using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public interface IBenchmarkRunInfoProvider
    {
        Task<BenchmarkRunInfo> GetRunInfoAsync(string path);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public interface IBenchmarkAnalyser
    {
        Task<BenchmarkResultAnalysis> AnalyseAsync(IEnumerable<BenchmarkInfo> benchmarks);
    }
}

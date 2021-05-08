using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Analysis;

namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IAnalyseBenchmarksExecutor
    {
        Task<BenchmarkResultAnalysis> ExecuteAsync(AnalyseBenchmarksExecutorArgs args);
    }
}

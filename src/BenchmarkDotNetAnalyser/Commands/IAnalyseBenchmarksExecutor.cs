using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IAnalyseBenchmarksExecutor
    {
        Task<bool> ExecuteAsync(AnalyseBenchmarksExecutorArgs args);
    }
}

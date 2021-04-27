using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IAggregateBenchmarksExecutor
    {
        Task<bool> ExecuteAsync(AggregateBenchmarksExecutorArgs args);
    }
}

namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IAggregateBenchmarksExecutorArgsReporter
    {
        void Report(AggregateBenchmarksExecutorArgs args);
    }
}

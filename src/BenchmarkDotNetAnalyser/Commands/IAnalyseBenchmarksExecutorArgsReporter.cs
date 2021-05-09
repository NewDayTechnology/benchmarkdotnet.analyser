namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IAnalyseBenchmarksExecutorArgsReporter
    {
        void Report(AnalyseBenchmarksExecutorArgs args);
    }
}

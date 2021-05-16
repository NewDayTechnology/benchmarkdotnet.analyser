namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IReportBenchmarksCommandValidator
    {
        void Validate(ReportBenchmarksCommand command);
    }
}
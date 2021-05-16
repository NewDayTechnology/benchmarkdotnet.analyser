namespace BenchmarkDotNetAnalyser.Reporting
{
    public interface IReporterProvider
    {
        IBenchmarksReportGenerator GetReporter(string kind);
        IBenchmarksReportGenerator GetReporter(ReportKind kind);
    }
}

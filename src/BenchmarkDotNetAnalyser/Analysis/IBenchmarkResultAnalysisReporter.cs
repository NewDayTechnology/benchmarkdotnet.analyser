namespace BenchmarkDotNetAnalyser.Analysis
{
    public interface IBenchmarkResultAnalysisReporter
    {
        bool Report(BenchmarkResultAnalysis results);
    }
}

using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkResult
    {
        public string FullName { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public string Parameters { get; set; }

        public decimal? Mean { get; set; }
        public decimal? Min { get; set; }
        public decimal? Q1 { get; set; }
        public decimal? Median { get; set; }
        public decimal? Q3 { get; set; }
        public decimal? Max { get; set; }
    }
}

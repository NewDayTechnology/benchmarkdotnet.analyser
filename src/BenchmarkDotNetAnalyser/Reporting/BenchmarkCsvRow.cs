using System;
using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Reporting
{
    [ExcludeFromCodeCoverage]
    public record BenchmarkCsvRow
    {
        public string FullName { get; init; }
        public string Namespace { get; init; }
        public string Type { get; init; }
        public string Method { get; init; }
        public string Parameters { get; init; }
        public DateTime Creation { get; init; }
        public string BuildNumber { get; init; }
        public string CommitSha { get; init; }
        public string BuildUrl { get; init; }
        public string BranchName { get; init; }
        public string Tag1 { get; init; }
        public string Tag2 { get; init; }
        public string Tag3 { get; init; }
        public string Tag4 { get; init; }
        public string Tag5 { get; init; }
        public string Tag6 { get; init; }
        public decimal? MinTime { get; init; }
        public decimal? MaxTime { get; init; }
        public decimal? MeanTime { get; init; }
        public decimal? MedianTime { get; init; }
        public decimal? Q1Time { get; init; }
        public decimal? Q3Time { get; init; }
        public decimal? Gen0Collections { get; init; }
        public decimal? Gen1Collections { get; init; }
        public decimal? Gen2Collections { get; init; }

        public decimal? TotalOps { get; init; }
        public decimal? BytesAllocatedPerOp { get; init; }
    }
}

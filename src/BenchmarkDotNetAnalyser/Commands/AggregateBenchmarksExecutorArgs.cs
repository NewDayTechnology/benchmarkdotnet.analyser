using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Commands
{
    [ExcludeFromCodeCoverage]
    public class AggregateBenchmarksExecutorArgs
    {
        private const string DefaultDataFileSuffix = "-report-full.json";

        public string NewBenchmarksPath { get; set; }
        public string AggregatedBenchmarksPath { get; set; }
        public string OutputAggregatesPath { get; set; }
        public int BenchmarkRuns { get; set; }
        public string DataFileSuffix { get; } = DefaultDataFileSuffix;
        public string BuildNumber { get; set; }
        public string BuildUri { get; set; }
        public string BranchName { get; set; }
        public string CommitSha { get; set; }
        public IList<string> Tags { get; set; }

    }
}

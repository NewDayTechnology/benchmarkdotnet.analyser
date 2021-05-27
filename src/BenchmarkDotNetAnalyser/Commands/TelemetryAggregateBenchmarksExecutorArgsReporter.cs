using System;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Commands
{
    public class TelemetryAggregateBenchmarksExecutorArgsReporter : IAggregateBenchmarksExecutorArgsReporter
    {
        private readonly ITelemetry _telemetry;

        public TelemetryAggregateBenchmarksExecutorArgsReporter(ITelemetry telemetry)
        {
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
        }

        public void Report(AggregateBenchmarksExecutorArgs args)
        {
            args.ArgNotNull(nameof(args));

            var msg = new[]
            {
                $"Aggregating from:\t{args.NewBenchmarksPath}",
                $"Aggregating with:\t{args.AggregatedBenchmarksPath}",
                $"Aggregating to:\t\t{args.OutputAggregatesPath}",
                $"Benchmark runs:\t\t{args.BenchmarkRuns:##.###}",
                $"Build:   \t\t{args.BuildNumber}",
                $"Build URI:\t\t{args.BuildUri}",
                $"Branch name:\t\t{args.BranchName}",
                $"Commit SHA:\t\t{args.CommitSha}",
                $"Tags:\t\t\t{args.Tags.NullToEmpty().Join(", ")}"

            }.Join(Environment.NewLine);

            _telemetry.Commentary(msg);
        }
    }
}

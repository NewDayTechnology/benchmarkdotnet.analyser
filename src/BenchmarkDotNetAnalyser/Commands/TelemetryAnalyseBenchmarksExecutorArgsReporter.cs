using System;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Commands
{
    public class TelemetryAnalyseBenchmarksExecutorArgsReporter : IAnalyseBenchmarksExecutorArgsReporter
    {
        private readonly ITelemetry _telemetry;

        public TelemetryAnalyseBenchmarksExecutorArgsReporter(ITelemetry telemetry)
        {
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
        }

        public void Report(AnalyseBenchmarksExecutorArgs args)
        {
            args.ArgNotNull(nameof(args));

            var msg = new[]
            {
                $"Aggregates:\t{args.AggregatesPath}",
                $"Tolerance:\t{args.Tolerance:P2}",
                $"Max Errors:\t{args.MaxErrors:##,###}",
                $"Statistic:\t{args.Statistic}",
            }.Join(Environment.NewLine);

            _telemetry.Commentary(msg);
        }
    }
}

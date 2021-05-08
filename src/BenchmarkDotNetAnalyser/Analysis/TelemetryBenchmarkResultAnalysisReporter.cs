using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class TelemetryBenchmarkResultAnalysisReporter : IBenchmarkResultAnalysisReporter
    {
        private readonly ITelemetry _telemetry;
        
        public TelemetryBenchmarkResultAnalysisReporter(ITelemetry telemetry)
        {
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
        }

        public bool Report(BenchmarkResultAnalysis result)
        {
            if (result.ArgNotNull(nameof(result)).MeetsRequirements)
            {
                _telemetry.Success("Benchmarks passed requirements.");
                return true;
            }

            var innerResults = result.InnerResults.NullToEmpty();
            if (innerResults.Any())
            {
                var lines = innerResults
                    .Where(r => !r.MeetsRequirements)
                    .Select(r => r.Message != null ? r.Message : r.BenchmarkName)
                    .Join(Environment.NewLine);

                var message = !string.IsNullOrWhiteSpace(result.Message)
                    ? $"{result.Message}{Environment.NewLine}These benchmark(s) failed performance:{Environment.NewLine}{lines}"
                    : $"These benchmark(s) failed performance:{Environment.NewLine}{lines}";

                _telemetry.Error(message);
            }

            return false;
        }
    }
}

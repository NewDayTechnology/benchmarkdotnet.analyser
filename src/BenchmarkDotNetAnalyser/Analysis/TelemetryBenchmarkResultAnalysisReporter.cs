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

            var message = result.InnerResults.NullToEmpty()
                .Where(r => !r.MeetsRequirements)
                .Select(r => r.Message != null ?  r.Message : r.BenchmarkName)
                .Join(Environment.NewLine)
                .Format($"These benchmark(s) failed performance:{Environment.NewLine}{{0}}");

            _telemetry.Error(message);
            return false;
        }
    }
}

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
            }
            else
            {
                var innerResults = result.InnerResults.NullToEmpty();
                if (innerResults.Any())
                {
                    var header = new[]
                    {
                        (!string.IsNullOrWhiteSpace(result.Message) ? result.Message : null),
                        "These benchmark(s) failed performance:"
                    };
                    
                    var lines = innerResults
                        .Where(r => !r.MeetsRequirements)
                        .Select(r => r.Message != null ? r.Message : r.BenchmarkName);
                    
                    var message = header.Concat(lines).Where(s => s != null).Join(Environment.NewLine);

                    _telemetry.Error(message);
                }
            }

            return result.MeetsRequirements;;
        }
    }
}

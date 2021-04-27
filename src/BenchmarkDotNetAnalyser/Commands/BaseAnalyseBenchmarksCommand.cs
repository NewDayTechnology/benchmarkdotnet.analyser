using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;
using McMaster.Extensions.CommandLineUtils;

namespace BenchmarkDotNetAnalyser.Commands
{
    public abstract class BaseAnalyseBenchmarksCommand
    {
        protected readonly ITelemetry Telemetry;
        protected readonly IBenchmarkInfoProvider InfoProvider;

        protected BaseAnalyseBenchmarksCommand(ITelemetry telemetry, IBenchmarkInfoProvider infoProvider)
        {
            Telemetry = telemetry.ArgNotNull(nameof(telemetry));
            InfoProvider = infoProvider.ArgNotNull(nameof(infoProvider));
        }

        [Option(CommandOptionType.SingleValue, Description = "The path of the aggregated dataset to analyse.", LongName = "aggregates", ShortName = "aggs")]
        public string AggregatesPath { get; set; }
        
        [Option(CommandOptionType.NoValue, LongName = "verbose", ShortName = "v", Description = "Emit verbose logging.")]
        public bool Verbose { get; set; }
    }
}

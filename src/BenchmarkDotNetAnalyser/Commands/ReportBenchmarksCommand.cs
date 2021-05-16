using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Instrumentation;
using BenchmarkDotNetAnalyser.Reporting;
using McMaster.Extensions.CommandLineUtils;

namespace BenchmarkDotNetAnalyser.Commands
{
    [Command("report", Description = "Build reports from a benchmark dataset.")]
    public class ReportBenchmarksCommand
    {
        private readonly IReporterProvider _reporterProvider;
        private readonly IReportBenchmarksCommandValidator _validator;
        private readonly ITelemetry _telemetry;

        public ReportBenchmarksCommand(ITelemetry telemetry,
            IReportBenchmarksCommandValidator validator,
            IReporterProvider reporterProvider)
        {
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
            _validator = validator.ArgNotNull(nameof(validator));
            _reporterProvider = reporterProvider.ArgNotNull(nameof(reporterProvider));
        }

        [Option(CommandOptionType.SingleValue, Description = "The path of the aggregated dataset to analyse.", LongName = "aggregates", ShortName = "aggs")]
        public string AggregatesPath { get; set; }
        
        [Option(CommandOptionType.MultipleValue, Description = "The reporting style. Optional, multiple reporters can be given.", LongName = "reporter", ShortName = "r")]
        public IList<string> Reporters { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The path for reports.", LongName = "output", ShortName = "out")]
        public string OutputPath { get; set; }

        [Option(CommandOptionType.MultipleValue, Description = "Filter by class or namespace. Optional, multiple filters can be given.", LongName = "filter", ShortName = "f")]
        public IList<string> Filters { get; set; }

        [Option(CommandOptionType.NoValue, LongName = "verbose", ShortName = "v", Description = "Emit verbose logging.")]
        public bool Verbose { get; set; }

        public async Task<int> OnExecuteAsync()
        {
            _telemetry.SetVerbosity(Verbose);
            
            try
            {
                _validator.Validate(this);

                return (await ExecuteAsync()).ToReturnCode();
            }
            catch (Exception ex)
            {
                _telemetry.Error(ex.Message);
                
                return false.ToReturnCode();
            }
        }

        private async Task<bool> ExecuteAsync()
        {
            _telemetry.Commentary("Generating reports...");
            var args = new ReportGenerationArgs()
            {
                AggregatesPath = this.AggregatesPath,
                OutputPath = this.OutputPath,
                Filters = this.Filters,
            };

            var reportCount = 0;
            foreach (var reporter in this.Reporters)
            {
                var gen = _reporterProvider.GetReporter(reporter);

                var outputs = await gen.GenerateAsync(args);
                
                foreach (var output in outputs)
                {
                    _telemetry.Commentary(output);
                    reportCount++;
                }
            }

            _telemetry.Commentary($"{reportCount} report(s) built.");
            _telemetry.Success("Done.");

            return true;
        }
    }
}

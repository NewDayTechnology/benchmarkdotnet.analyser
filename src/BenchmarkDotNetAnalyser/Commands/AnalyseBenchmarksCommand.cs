using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;
using McMaster.Extensions.CommandLineUtils;

namespace BenchmarkDotNetAnalyser.Commands
{
    [Command("analyse", Description = "Analyse a benchmark dataset for performance degradation.")]
    public class AnalyseBenchmarksCommand : BaseAnalyseBenchmarksCommand
    {
        private readonly IBenchmarkResultAnalysisReporter _reporter;
        private readonly IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand> _validator;
        private readonly IAnalyseBenchmarksExecutor _executor;

        public AnalyseBenchmarksCommand(ITelemetry telemetry, IBenchmarkInfoProvider infoProvider,
                                        IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand> validator,
                                        IAnalyseBenchmarksExecutor executor, IBenchmarkResultAnalysisReporter reporter) 
            : base(telemetry, infoProvider)
        {
            _reporter = reporter.ArgNotNull(nameof(reporter));
            _validator = validator.ArgNotNull(nameof(validator));
            _executor = executor.ArgNotNull(nameof(executor));
        }
        
        [Option(CommandOptionType.SingleValue, Description = "Tolerance of errors from baseline performance.", LongName = "tolerance", ShortName = "tol")]
        public string Tolerance { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The maximum number of failures to tolerate.", LongName = "maxerrors", ShortName = "max")]
        public string MaxErrors { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The result statistic to analyse.", LongName = "statistic", ShortName = "stat")]
        public string Statistic { get; set; }

        [Option(CommandOptionType.MultipleValue, Description = "Filter by class or namespace. Optional.", LongName = "filter", ShortName = "f")]
        public IList<string> Filters { get; set; }

        public async Task<int> OnExecuteAsync()
        {
            Telemetry.SetVerbosity(Verbose);

            try
            {
                _validator.Validate(this);

                return (await ExecuteAsync()).ToReturnCode();
            }
            catch (Exception ex)
            {
                Telemetry.Error(ex.Message);

                return false.ToReturnCode();
            }
        }

        private async Task<bool> ExecuteAsync()
        {
            var args = new AnalyseBenchmarksExecutorArgs()
            {
                Verbose = this.Verbose,
                AggregatesPath = this.AggregatesPath,
                Tolerance = this.Tolerance.ToPercentageDecimal(),
                MaxErrors = this.MaxErrors.ToInt(),
                Statistic = this.Statistic,
                Filters = this.Filters,
            };

            var analysisResult = await _executor.ExecuteAsync(args);

            return _reporter.Report(analysisResult);
        }
    }
}

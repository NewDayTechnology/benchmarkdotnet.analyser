using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Instrumentation;
using McMaster.Extensions.CommandLineUtils;

namespace BenchmarkDotNetAnalyser.Commands
{
    [Command("aggregate", Description = "Aggregate benchmark results into a single dataset")]
    public class AggregateBenchmarksCommand
    {
        private readonly ITelemetry _telemetry;
        private readonly IAggregateBenchmarksCommandValidator _validator;
        private readonly IAggregateBenchmarksExecutor _executor;

        public AggregateBenchmarksCommand(ITelemetry telemetry,
                                          IAggregateBenchmarksCommandValidator validator,
                                          IAggregateBenchmarksExecutor executor)
        {
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
            _validator = validator.ArgNotNull(nameof(validator));
            _executor = executor.ArgNotNull(nameof(executor));
        }

        [Option(CommandOptionType.SingleValue, Description = "The path containing new benchmarks results.", LongName = "new", ShortName = "new")]
        public string NewBenchmarksPath { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The path containing the dataset to roll into.", LongName = "aggregates", ShortName = "aggs")]
        public string AggregatedBenchmarksPath { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The path for the new dataset.", LongName = "output", ShortName = "out")]
        public string OutputAggregatesPath { get; set; }
        
        [Option(CommandOptionType.SingleValue, Description = "The number of benchmark runs to keep when aggregating.", LongName = "runs", ShortName = "runs")]
        public string BenchmarkRuns { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The file name suffix for data files. Optional. Default: " + AggregateBenchmarksCommandValidator.DefaultDataFileSuffix, LongName = "datafilesuffix", ShortName = "datafilesuffix")]
        public string DataFileSuffix { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The new build's URL. Optional.", LongName = "build", ShortName = "build")]
        public string BuildUri { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The new build's branch name. Optional.", LongName = "branch", ShortName = "branch")]
        public string BranchName { get; set; }

        [Option(CommandOptionType.MultipleValue, Description = "A tag for the new build. Optional.", LongName = "tag", ShortName = "t")]
        public IList<string> Tags { get; set; }

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
            var args = new AggregateBenchmarksExecutorArgs()
            {
                OutputAggregatesPath = this.OutputAggregatesPath,
                BranchName = this.BranchName,
                BenchmarkRuns = this.BenchmarkRuns.ToInt(),
                AggregatedBenchmarksPath = this.AggregatedBenchmarksPath,
                BuildUri = this.BuildUri,
                DataFileSuffix = this.DataFileSuffix,
                NewBenchmarksPath = this.NewBenchmarksPath,
                Tags = this.Tags,
            };
            
            return await _executor.ExecuteAsync(args);

        }
    }
}

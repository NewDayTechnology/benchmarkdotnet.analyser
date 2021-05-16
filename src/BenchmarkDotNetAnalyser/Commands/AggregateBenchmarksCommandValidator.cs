using System;
using BenchmarkDotNetAnalyser.IO;

namespace BenchmarkDotNetAnalyser.Commands
{
    public class AggregateBenchmarksCommandValidator : IAggregateBenchmarksCommandValidator
    {
        
        public void Validate(AggregateBenchmarksCommand command) => command.ArgNotNull(nameof(command))
                                                                           .PipeDo(ValidateAndFixParameters);

        private void ValidateAndFixParameters(AggregateBenchmarksCommand command)
        {
            command.NewBenchmarksPath = command.NewBenchmarksPath
                .InvalidOpArg(String.IsNullOrWhiteSpace, $"The {command.GetCommandOptionName(nameof(command.NewBenchmarksPath))} parameter is missing.")
                .ResolveWorkingPath()
                .AssertPathExists();
            
            command.AggregatedBenchmarksPath = command.AggregatedBenchmarksPath
                .InvalidOpArg(String.IsNullOrWhiteSpace, $"The {command.GetCommandOptionName(nameof(command.AggregatedBenchmarksPath))} parameter is missing.")
                .ResolveWorkingPath()
                .AssertPathExists();

            command.OutputAggregatesPath = command.OutputAggregatesPath
                .InvalidOpArg(String.IsNullOrWhiteSpace, $"The {command.GetCommandOptionName(nameof(command.OutputAggregatesPath))} parameter is missing.")
                .ResolveWorkingPath()
                .GetOrCreateFullPath();

            command.BenchmarkRuns = command.BenchmarkRuns.PipeIfNotNull(x =>
            {
                var ok = int.TryParse(x, out var y);
                ok.InvalidOpArg(x => !x, $"The {command.GetCommandOptionName(nameof(command.BenchmarkRuns))} parameter must be numeric.");
                y.InvalidOpArg(x => x < 1, $"The {command.GetCommandOptionName(nameof(command.BenchmarkRuns))} parameter must have be least 1");
                return x;
            }, "1");

            command.BuildUri = command.BuildUri.PipeIfNotNull(u => u.InvalidOpArg(x => !Uri.TryCreate(x, UriKind.Absolute, out var _), $"The {command.GetCommandOptionName(nameof(command.BuildUri))} parameter must be a URI."));
        }
    }
}

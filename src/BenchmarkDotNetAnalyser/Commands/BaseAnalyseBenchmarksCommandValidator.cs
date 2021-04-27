using System;
using BenchmarkDotNetAnalyser.IO;

namespace BenchmarkDotNetAnalyser.Commands
{
    public abstract class BaseAnalyseBenchmarksCommandValidator 
    {
        public virtual void Validate(BaseAnalyseBenchmarksCommand command) => command.ArgNotNull(nameof(command))
                                                                                     .PipeDo(ValidateAndFixParameters);

        private void ValidateAndFixParameters(BaseAnalyseBenchmarksCommand command)
        {
            command.AggregatesPath = command.AggregatesPath
                .InvalidOpArg(String.IsNullOrWhiteSpace, $"The {command.GetCommandOptionName(nameof(command.AggregatesPath))} parameter is missing.")
                .ResolveWorkingPath()
                .AssertPathExists();
        }
    }
}

using System;
using System.Linq;
using BenchmarkDotNetAnalyser.IO;
using BenchmarkDotNetAnalyser.Reporting;

namespace BenchmarkDotNetAnalyser.Commands
{
    public class ReportBenchmarksCommandValidator : IReportBenchmarksCommandValidator
    {
        public void Validate(ReportBenchmarksCommand command) => command.ArgNotNull(nameof(command))
            .PipeDo(ValidateAndFixParameters);

        private void ValidateAndFixParameters(ReportBenchmarksCommand command)
        {
            command.AggregatesPath = command.AggregatesPath
                .InvalidOpArg(String.IsNullOrWhiteSpace, $"The {command.GetCommandOptionName(nameof(command.AggregatesPath))} parameter is missing.")
                .ResolveWorkingPath()
                .AssertPathExists();

            command.OutputPath = command.OutputPath
                .InvalidOpArg(String.IsNullOrWhiteSpace, $"The {command.GetCommandOptionName(nameof(command.OutputPath))} parameter is missing.")
                .ResolveWorkingPath()
                .GetOrCreateFullPath();
            
            command.Reporters = command.Reporters.PipeIfNotNull(reporters =>
            {
                var xs = Enum.GetValues(typeof(ReportKind))
                    .OfType<ReportKind>()
                    .Select(e => e.ToString())
                    .ToList();
                
                foreach (var reporter in reporters)
                {
                    reporter.InvalidOpArg(s => !xs.Contains(s, StringComparer.InvariantCultureIgnoreCase), $"{reporter} is an invalid reporter. Valid options are: {xs.Join(", ")}");
                }
                
                return reporters;
            }, ReportKind.Csv.ToString().Singleton().ToList());
        }
    }
}

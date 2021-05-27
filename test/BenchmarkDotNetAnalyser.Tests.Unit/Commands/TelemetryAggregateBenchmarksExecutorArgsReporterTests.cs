using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class TelemetryAggregateBenchmarksExecutorArgsReporterTests
    {
        [Property(Verbose = true, Arbitrary = new[] { typeof(AlphanumericStringArbitrary)})]
        public bool Report_DetailsSentToTelemetry(string newAggsPath, string aggsPath,
            string outputPath, PositiveInt runs, string branch, string buildUrl, string commit)
        {
            var telemetry = Substitute.For<ITelemetry>();

            var args = new AggregateBenchmarksExecutorArgs()
            {
                NewBenchmarksPath = newAggsPath,
                AggregatedBenchmarksPath = aggsPath,
                OutputAggregatesPath = outputPath,
                BenchmarkRuns = runs.Get,
                BranchName = branch,
                CommitSha = commit,
                BuildUri = buildUrl,
            };

            new TelemetryAggregateBenchmarksExecutorArgsReporter(telemetry).Report(args);

            Func<string, bool> predicate = x =>
            {
                var splits = x.Split(Environment.NewLine);
                return splits.All(x => x.Length > 0);
            };

            telemetry.Received(1).Commentary(Arg.Is<string>(s => predicate(s)));
            telemetry.Received(1).Commentary(Arg.Is<string>(s => s.Contains(newAggsPath)));
            telemetry.Received(1).Commentary(Arg.Is<string>(s => s.Contains(aggsPath)));
            telemetry.Received(1).Commentary(Arg.Is<string>(s => s.Contains(outputPath)));
            telemetry.Received(1).Commentary(Arg.Is<string>(s => s.Contains(branch)));
            telemetry.Received(1).Commentary(Arg.Is<string>(s => s.Contains(buildUrl)));

            return true;
        }

        [Property(Verbose = true)]
        public bool Report_TagsSentToTelemetry(PositiveInt tagCount)
        {
            var telemetry = Substitute.For<ITelemetry>();
            var tags = Enumerable.Range(1, tagCount.Get).Select(x => $"Tag{x}").ToList();

            var args = new AggregateBenchmarksExecutorArgs()
            {
                Tags = tags,
            };

            new TelemetryAggregateBenchmarksExecutorArgsReporter(telemetry).Report(args);

            Func<string, bool> predicate = x =>
            {
                var tagSegments = x.Split(Environment.NewLine)
                                        .FirstOrDefault(s => s.StartsWith("Tags:"))
                                        ?.Split("\t").Last().Split(", ");

                return tagSegments.SequenceEqual(tags);
            };

            telemetry.Received(1).Commentary(Arg.Is<string>(s => predicate(s)));
            
            return true;
        }
    }
}

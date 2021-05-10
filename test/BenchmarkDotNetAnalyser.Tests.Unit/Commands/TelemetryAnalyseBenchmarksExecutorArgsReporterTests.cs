using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class TelemetryAnalyseBenchmarksExecutorArgsReporterTests
    {
        
        [Property(Verbose = true, Arbitrary = new[] { typeof(AlphanumericStringArbitrary)})]
        public bool Report_TelemetryReceivesArgsDetails(string aggs, string statistic, PositiveInt maxErrors, PositiveInt tol)
        {
            var args = new AnalyseBenchmarksExecutorArgs()
            {
                AggregatesPath = aggs,
                MaxErrors = maxErrors.Get,
                Statistic = statistic,
                Tolerance = (tol.Get * 1.1M),
            };

            var telemetry = Substitute.For<ITelemetry>();
            
            new TelemetryAnalyseBenchmarksExecutorArgsReporter(telemetry).Report(args);
            
            Func<string, bool> predicate = x =>
            {
                var splits = x.Split(Environment.NewLine);
                return splits.All(x => x.Length > 0);
            };

            telemetry.Received(1).Commentary(Arg.Is<string>(s => predicate(s)));
            telemetry.Received(1).Commentary(Arg.Is<string>(s => s.Contains(aggs)));
            telemetry.Received(1).Commentary(Arg.Is<string>(s => s.Contains(statistic)));
            telemetry.Received(0).Info(Arg.Any<string>());

            return true;
        }
    }
}

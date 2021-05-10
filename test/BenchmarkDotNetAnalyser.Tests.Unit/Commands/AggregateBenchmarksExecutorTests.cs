using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Aggregation;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using BenchmarkDotNetAnalyser.IO;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class AggregateBenchmarksExecutorTests
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        [InlineData(7, 1)]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(3, 0)]
        [InlineData(4, 0)]
        [InlineData(7, 0)]
        public async Task ExecuteAsync_BenchmarksAggregated_AggregateWritten(int aggregateCount, int newFileCount)
        {
            var newFilePaths = newFileCount > 0 ? new[] {"File1" } : new string[1];
            var aggregatedBenchmarks = Enumerable.Range(1, aggregateCount).Select(i => new BenchmarkInfo()).ToList();
            
            var telemetry = Substitute.For<ITelemetry>();
            
            var fileFinder = Substitute.For<IFileFinder>();
            fileFinder.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(newFilePaths);

            var runInfoProvider = Substitute.For<IBenchmarkRunInfoProvider>();
            runInfoProvider.GetRunInfoAsync(Arg.Any<string>())
                .Returns(x => Task.FromResult(new BenchmarkRunInfo() { Creation = DateTimeOffset.UtcNow}));

            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            
            infoProvider.GetBenchmarkInfosAsync(Arg.Any<string>()).Returns(aggregatedBenchmarks);

            infoProvider.WriteBenchmarkInfosAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<BenchmarkInfo>>())
                .Returns(_ => Task.FromResult(true));
            
            var aggregator = Substitute.For<IBenchmarkAggregator>();
            aggregator.Aggregate(Arg.Any<BenchmarkAggregationOptions>(), Arg.Any<IEnumerable<BenchmarkInfo>>(), Arg.Any<BenchmarkInfo>())
                .Returns(ci =>
                {
                    var xs = ((IEnumerable<BenchmarkInfo>) ci[1]).ToList();
                    var x = ci[2] as BenchmarkInfo;
                    xs.Add(x);
                    return xs;
                    
                });
            
            var exec = new AggregateBenchmarksExecutor(telemetry, fileFinder, runInfoProvider, infoProvider, aggregator);
            
            var args = new AggregateBenchmarksExecutorArgs();

            var r = await exec.ExecuteAsync(args);

            r.Should().BeTrue();

            aggregator.Received(1).Aggregate(Arg.Is<BenchmarkAggregationOptions>(x => x.PreservePinned),
                Arg.Any<IEnumerable<BenchmarkInfo>>(), Arg.Any<BenchmarkInfo>());

            await infoProvider.Received(1).WriteBenchmarkInfosAsync(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Is<IEnumerable<BenchmarkInfo>>(xs => xs.Count() == aggregatedBenchmarks.Count + 1));
        }

        
        [Fact]
        public async Task ExecuteAsync_Success_ConsoleMessagesSent()
        {
            var telemetry = Substitute.For<ITelemetry>();
            
            var fileFinder = Substitute.For<IFileFinder>();
            fileFinder.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new string[0]);

            var runInfoProvider = Substitute.For<IBenchmarkRunInfoProvider>();
            runInfoProvider.GetRunInfoAsync(Arg.Any<string>())
                .Returns(x => Task.FromResult(new BenchmarkRunInfo()));

            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            infoProvider.GetBenchmarkInfosAsync(Arg.Any<string>()).Returns(new List<BenchmarkInfo>());
            infoProvider.WriteBenchmarkInfosAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<BenchmarkInfo>>())
                .Returns(_ => Task.FromResult(true));
            
            var aggregator = Substitute.For<IBenchmarkAggregator>();
            aggregator.Aggregate(Arg.Any<BenchmarkAggregationOptions>(), Arg.Any<IEnumerable<BenchmarkInfo>>(), Arg.Any<BenchmarkInfo>())
                .Returns(new List<BenchmarkInfo>());


            var exec = new AggregateBenchmarksExecutor(telemetry, fileFinder, runInfoProvider, infoProvider, aggregator);
            
            var args = new AggregateBenchmarksExecutorArgs();

            var r = await exec.ExecuteAsync(args);

            r.Should().BeTrue();

            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "Getting new benchmark..."));
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "Getting prior benchmarks..."));
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "Aggregating..."));
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "Writing aggregations..."));
            telemetry.Received(1).Success(Arg.Is<string>(x => x == "Aggregation complete."));
        }

    }
}

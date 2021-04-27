using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class AnalyseBenchmarksExecutorTests
    {
        [Fact]
        public async Task ExecuteAsync_EmptyBenchmarks_ReturnsFalse()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            var analyser = Substitute.For<IBenchmarkAnalyser>();

            var exec = new AnalyseBenchmarksExecutor(telemetry, infoProvider, _ => analyser);

            var args = new AnalyseBenchmarksExecutorArgs();

            var result = await exec.ExecuteAsync(args);

            result.Should().BeFalse();
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "Getting benchmarks..."));
            telemetry.Received(1).Warning(Arg.Is<string>(x => x == "No benchmarks found."));
        } 


        [Fact]
        public async Task ExecuteAsync_SingleBenchmark_MeetsRequirements_ReturnsTrue()
        {
            var benchmarks = new[] {new BenchmarkInfo() {} };
            var analysis = new BenchmarkResultAnalysis() { MeetsRequirements = true };

            var telemetry = Substitute.For<ITelemetry>();
            
            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            infoProvider.GetBenchmarkInfosAsync(Arg.Any<string>())
                .Returns(_ => benchmarks);

            var analyser = Substitute.For<IBenchmarkAnalyser>();
            analyser.AnalyseAsync(Arg.Any<IEnumerable<BenchmarkInfo>>())
                .Returns(Task.FromResult(analysis));

            var exec = new AnalyseBenchmarksExecutor(telemetry, infoProvider, _ => analyser);

            var args = new AnalyseBenchmarksExecutorArgs();

            var result = await exec.ExecuteAsync(args);

            result.Should().BeTrue();
            telemetry.Received(1).Info(Arg.Is<string>(x => x == "Analysing benchmarks..."));
            telemetry.Received(1).Info(Arg.Is<string>(x => x == "Analysis done."));
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "Getting benchmarks..."));
            telemetry.Received(1).Commentary(Arg.Is<string>(x => x.EndsWith("benchmark(s) found.")));
            telemetry.Received(0).Warning(Arg.Is<string>(x => x == "No benchmarks found."));
        } 

        [Fact]
        public async Task ExecuteAsync_SingleBenchmark_DoesNot_MeetsRequirements_ReturnsFalse()
        {
            var benchmarks = new[] {new BenchmarkInfo() {} };
            var analysis = new BenchmarkResultAnalysis() { MeetsRequirements = false };

            var telemetry = Substitute.For<ITelemetry>();
            
            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            infoProvider.GetBenchmarkInfosAsync(Arg.Any<string>())
                .Returns(_ => benchmarks);

            var analyser = Substitute.For<IBenchmarkAnalyser>();
            analyser.AnalyseAsync(Arg.Any<IEnumerable<BenchmarkInfo>>())
                .Returns(Task.FromResult(analysis));

            var exec = new AnalyseBenchmarksExecutor(telemetry, infoProvider, _ => analyser);

            var args = new AnalyseBenchmarksExecutorArgs();

            var result = await exec.ExecuteAsync(args);

            result.Should().BeFalse();
            telemetry.Received(1).Info(Arg.Is<string>(x => x == "Analysing benchmarks..."));
            telemetry.Received(1).Info(Arg.Is<string>(x => x == "Analysis done."));
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "Getting benchmarks..."));
            telemetry.Received(1).Commentary(Arg.Is<string>(x => x.EndsWith("benchmark(s) found.")));
            telemetry.Received(0).Warning(Arg.Is<string>(x => x == "No benchmarks found."));
        } 
    }
}

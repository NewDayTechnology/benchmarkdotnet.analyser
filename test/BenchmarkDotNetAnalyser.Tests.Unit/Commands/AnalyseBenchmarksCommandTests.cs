using System;
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
    public class AnalyseBenchmarksCommandTests
    {
        [Fact]
        public async Task OnExecuteAsync_ValidationError_ReturnsError()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            var validator = Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>();
            validator.When(x => x.Validate(Arg.Any<AnalyseBenchmarksCommand>()))
                .Do(_ =>
                {
                    throw new InvalidOperationException();
                });
            var executor = Substitute.For<IAnalyseBenchmarksExecutor>();

            var cmd = new AnalyseBenchmarksCommand(telemetry, infoProvider, validator, executor);

            var rc = await cmd.OnExecuteAsync();

            rc.Should().BeGreaterThan(0);
            executor.Received(0);
        }

        [Fact]
        public async Task OnExecuteAsync_NoValidationError_ExecuteOk_ReturnsOk()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            var validator = Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>();
            var executor = Substitute.For<IAnalyseBenchmarksExecutor>();
            var analysisResult = new BenchmarkResultAnalysis() {MeetsRequirements = true};
            executor.ExecuteAsync(Arg.Any<AnalyseBenchmarksExecutorArgs>()).Returns(analysisResult);

            var cmd = new AnalyseBenchmarksCommand(telemetry, infoProvider, validator, executor)
            {
                MaxErrors = 1.ToString(),
                Tolerance = 0.0.ToString(),
            };

            var rc = await cmd.OnExecuteAsync();

            rc.Should().Be(0);
            executor.Received(1);
            telemetry.Received(0);
        }

        [Fact]
        public async Task OnExecuteAsync_NoValidationError_ExecuteError_ReturnsError()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var infoProvider = Substitute.For<IBenchmarkInfoProvider>();
            var validator = Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>();
            var executor = Substitute.For<IAnalyseBenchmarksExecutor>();
            var analysisResult = new BenchmarkResultAnalysis() {MeetsRequirements = false};
            executor.ExecuteAsync(Arg.Any<AnalyseBenchmarksExecutorArgs>()).Returns(analysisResult);

            var cmd = new AnalyseBenchmarksCommand(telemetry, infoProvider, validator, executor)
            {
                MaxErrors = 1.ToString(),
                Tolerance = 0.0.ToString(),
            };

            var rc = await cmd.OnExecuteAsync();

            rc.Should().BeGreaterThan(0);
            executor.Received(1);
            telemetry.Received(1);
        }
    }
}

using System;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using Shouldly;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class AggregateBenchmarksCommandTests
    {
        [Fact]
        public async Task OnExecuteAsync_ValidationError_ReturnsError()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var validator = Substitute.For<IAggregateBenchmarksCommandValidator>();
            validator.When(x => x.Validate(Arg.Any<AggregateBenchmarksCommand>()))
                .Do(_ =>
                {
                    throw new InvalidOperationException();
                });
            var executor = Substitute.For<IAggregateBenchmarksExecutor>();

            var cmd = new AggregateBenchmarksCommand(telemetry, validator, executor);

            var rc = await cmd.OnExecuteAsync();

            rc.ShouldBeGreaterThan(0);
            executor.Received(0);
        }

        [Fact]
        public async Task OnExecuteAsync_NoValidationError_ExecuteOk_ReturnsOk()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var validator = Substitute.For<IAggregateBenchmarksCommandValidator>();
            var executor = Substitute.For<IAggregateBenchmarksExecutor>();
            executor.ExecuteAsync(Arg.Any<AggregateBenchmarksExecutorArgs>()).Returns(true);

            var cmd = new AggregateBenchmarksCommand(telemetry, validator, executor)
            {
                BenchmarkRuns = 1.ToString(),
            };

            var rc = await cmd.OnExecuteAsync();

            rc.ShouldBe(0);
            executor.Received(1);
            telemetry.Received(0);
        }

        [Fact]
        public async Task OnExecuteAsync_NoValidationError_ExecuteError_ReturnsError()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var validator = Substitute.For<IAggregateBenchmarksCommandValidator>();
            var executor = Substitute.For<IAggregateBenchmarksExecutor>();
            executor.ExecuteAsync(Arg.Any<AggregateBenchmarksExecutorArgs>()).Returns(false);

            var cmd = new AggregateBenchmarksCommand(telemetry, validator, executor)
            {
                BenchmarkRuns = 1.ToString(),
            };

            var rc = await cmd.OnExecuteAsync();

            rc.ShouldBeGreaterThan(0);
            executor.Received(1);
            telemetry.Received(1);
        }
    }
}

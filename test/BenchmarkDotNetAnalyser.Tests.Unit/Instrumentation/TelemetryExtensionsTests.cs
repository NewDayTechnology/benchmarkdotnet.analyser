using System;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Instrumentation;
using Shouldly;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Instrumentation
{
    public class TelemetryExtensionsTests
    {
        [Fact]
        public async Task InvokeWithLoggingAsync_ResultReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var log = new TelemetryEntry(ConsoleColor.Blue, false, "msg");
            var response = 1;

            var result = await telemetry.InvokeWithLoggingAsync(log, () => Task.FromResult(response));

            result.ShouldBe(response);

            telemetry.Received(2).Write(Arg.Any<TelemetryEntry>());
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "done." && x.AddLineBreak));
        }

        [Fact]
        public void InvokeWithLoggingAsync_ExceptionThrown()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var log = new TelemetryEntry(ConsoleColor.Blue, false, "msg");
            
            Func<Task<int>> func = () => throw new Exception();

            Func<Task<int>> f = async () => await telemetry.InvokeWithLoggingAsync(log, func);

            f.ShouldThrowAsync<Exception>();
            telemetry.Received(2).Write(Arg.Any<TelemetryEntry>());
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(x => x.Message == "" && x.AddLineBreak));
        }

        [Fact]
        public void InvokeWithLogging_ResultReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var log = new TelemetryEntry(ConsoleColor.Blue, false, "msg");
            var response = 1;

            var result = telemetry.InvokeWithLogging(log, () => response);

            result.ShouldBe(response);

            telemetry.Received(2).Write(Arg.Any<TelemetryEntry>());
        }

        [Fact]
        public void InvokeWithLogging_ExceptionThrown()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var log = new TelemetryEntry(ConsoleColor.Blue, false, "msg");
            
            Func<int> func = () => throw new Exception();

            Action f = () => telemetry.InvokeWithLogging(log, func);

            f.ShouldThrow<Exception>();
            telemetry.Received(2).Write(Arg.Any<TelemetryEntry>());
        }


    }
}

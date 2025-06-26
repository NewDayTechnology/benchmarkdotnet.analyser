using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;
using Shouldly;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Analysis
{
    public class BaselineDevianceGroupAnalyserTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Analyse_LatestWithinTolerance_ReturnsTrue(int resultCount)
        {
            var telemetry = Substitute.For<ITelemetry>();
            var accessors = Substitute.For<IBenchmarkStatisticAccessorProvider>();
            var analyser = new BaselineDevianceGroupAnalyser(telemetry, accessors, 0.0m, null);

            var now = DateTime.UtcNow;
            var times = Enumerable.Range(1, resultCount)
                .Select(i => now.AddDays(-i))
                .ToList();

            var benchmarkResults = times.Select((t, i) =>
            {
                var ri = new BenchmarkRunInfo() {Creation = t};
                var rr = new BenchmarkResult() {MeanTime = i+1};
                return (ri, rr);
            }).ToList();
                
            var group = new BenchmarkResultGroup()
            {
                Results = benchmarkResults,
            };

            var result = analyser.Analyse(group).ToList();

            result.Count.ShouldBe(1);
            result[0].MeetsRequirements.ShouldBeTrue();
            result[0].Message.ShouldBeNull();
            telemetry.Received(0).Write(Arg.Any<TelemetryEntry>());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void Analyse_LatestExceedsTolerance_ReturnsFalse(int resultCount)
        {
            var telemetry = Substitute.For<ITelemetry>();
            var accessors = Substitute.For<IBenchmarkStatisticAccessorProvider>();
            accessors.GetAccessor(Arg.Any<string>()).Returns((BenchmarkResult br) => br.MeanTime.GetValueOrDefault());
            var analyser = new BaselineDevianceGroupAnalyser(telemetry, accessors, 0.0m, null);

            var now = DateTime.UtcNow;
            var times = Enumerable.Range(1, resultCount)
                .Select(i => now.AddDays(-i))
                .Reverse()
                .ToList();

            var benchmarkResults = times.Select((t, i) =>
            {
                var ri = new BenchmarkRunInfo() {Creation = t};
                var rr = new BenchmarkResult() {MeanTime = i+1};
                return (ri, rr);
            }).ToList();
                
            var group = new BenchmarkResultGroup()
            {
                Results = benchmarkResults,
            };

            var result = analyser.Analyse(group).ToList();

            result.Count.ShouldBe(1);
            result[0].MeetsRequirements.ShouldBeFalse();
            result[0].Message.ShouldNotBeNullOrWhiteSpace();
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(t => t.AddLineBreak && t.IsVerbose && !string.IsNullOrWhiteSpace(t.Message)));
        }
    }
}

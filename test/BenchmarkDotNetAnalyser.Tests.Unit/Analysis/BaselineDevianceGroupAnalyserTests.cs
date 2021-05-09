using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;
using FluentAssertions;
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
                var rr = new BenchmarkResult() {Mean = i+1};
                return (ri, rr);
            }).ToList();
                
            var group = new BenchmarkResultGroup()
            {
                Results = benchmarkResults,
            };

            var result = analyser.Analyse(group).ToList();

            result.Count.Should().Be(1);
            result[0].MeetsRequirements.Should().BeTrue();
            result[0].Message.Should().BeNull();
            telemetry.Received(0).Write(Arg.Any<TelemetryEntry>());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void Analyse_LatestExceedsTolerance_ReturnsFalse(int resultCount)
        {
            var telemetry = Substitute.For<ITelemetry>();
            var accessors = Substitute.For<IBenchmarkStatisticAccessorProvider>();
            accessors.GetAccessor(Arg.Any<string>()).Returns((BenchmarkResult br) => br.Mean.GetValueOrDefault());
            var analyser = new BaselineDevianceGroupAnalyser(telemetry, accessors, 0.0m, null);

            var now = DateTime.UtcNow;
            var times = Enumerable.Range(1, resultCount)
                .Select(i => now.AddDays(-i))
                .Reverse()
                .ToList();

            var benchmarkResults = times.Select((t, i) =>
            {
                var ri = new BenchmarkRunInfo() {Creation = t};
                var rr = new BenchmarkResult() {Mean = i+1};
                return (ri, rr);
            }).ToList();
                
            var group = new BenchmarkResultGroup()
            {
                Results = benchmarkResults,
            };

            var result = analyser.Analyse(group).ToList();

            result.Count.Should().Be(1);
            result[0].MeetsRequirements.Should().BeFalse();
            result[0].Message.Should().NotBeNullOrWhiteSpace();
            telemetry.Received(1).Write(Arg.Is<TelemetryEntry>(t => t.AddLineBreak && t.IsVerbose && !string.IsNullOrWhiteSpace(t.Message)));
        }
    }
}

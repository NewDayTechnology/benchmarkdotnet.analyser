using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Instrumentation;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Analysis
{
    public class TelemetryBenchmarkResultAnalysisReporterTests
    {
        [Fact]
        public void Report_MeetsRequirementsTrue_ErrorReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var reporter = new TelemetryBenchmarkResultAnalysisReporter(telemetry);

            var analysis = new BenchmarkResultAnalysis()
            {
                MeetsRequirements = true,
                Message = "success"
            };

            var result = reporter.Report(analysis);
            result.Should().BeTrue();
            telemetry.Received(0).Error(Arg.Any<string>());
            telemetry.Received(1).Success(Arg.Is<string>(s => !string.IsNullOrWhiteSpace(s)));
        }


        [Fact]
        public void Report_MeetsRequirementsFalse_EmptyInnerResults_ErrorReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var reporter = new TelemetryBenchmarkResultAnalysisReporter(telemetry);

            var analysis = new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
            };

            var result = reporter.Report(analysis);

            result.Should().BeFalse();
            telemetry.Received(0).Error(Arg.Any<string>());
            telemetry.Received(0).Success(Arg.Any<string>());
        }

        
        [Fact]
        public void Report_MeetsRequirementsFalse_NullMessage_InnerMessageReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var reporter = new TelemetryBenchmarkResultAnalysisReporter(telemetry);

            var msgs = Enumerable.Range(1, 3).Select(_ => Guid.NewGuid().ToString()).ToList();
            var innerResults = msgs.Select(msg => new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
                Message = msg
            }).ToList();

            var analysis = new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
                Message = null,
                InnerResults = innerResults,
            };

            var result = reporter.Report(analysis);

            result.Should().BeFalse();
            telemetry.Received(0).Success(Arg.Any<string>());
            telemetry.Received(1).Error(Arg.Is<string>(s => msgs.Any(s.Contains)));
        }

        [Fact]
        public void Report_MeetsRequirementsFalse_InnerMessageNotReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var reporter = new TelemetryBenchmarkResultAnalysisReporter(telemetry);

            var msgs = Enumerable.Range(1, 3).Select(_ => Guid.NewGuid().ToString()).ToList();
            var innerResults = msgs.Select(msg => new BenchmarkResultAnalysis()
            {
                MeetsRequirements = true,
                Message = msg
            }).ToList();

            var analysis = new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
                InnerResults = innerResults,
            };

            var result = reporter.Report(analysis);

            result.Should().BeFalse();
            telemetry.Received(0).Success(Arg.Any<string>());
            telemetry.Received(1).Error(Arg.Any<string>());
            telemetry.Received(0).Error(Arg.Is<string>(s => msgs.Any(s.Contains)));
        }

        
        [Fact]
        public void Report_MeetsRequirementsFalse_WhitespaceMessage_InnerMessageReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var reporter = new TelemetryBenchmarkResultAnalysisReporter(telemetry);

            var msgs = Enumerable.Range(1, 3).Select(_ => Guid.NewGuid().ToString()).ToList();
            var innerResults = msgs.Select(msg => new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
                Message = msg
            }).ToList();

            var analysis = new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
                Message = "   ",
                InnerResults = innerResults,
            };

            var result = reporter.Report(analysis);

            result.Should().BeFalse();
            telemetry.Received(0).Success(Arg.Any<string>());
            telemetry.Received(1).Error(Arg.Is<string>(s => msgs.Any(s.Contains)));
            telemetry.Received(1).Error(Arg.Is<string>(s => !s.Contains(analysis.Message)));
        }

        [Fact]
        public void Report_MeetsRequirementsFalse_NonNullMessage_InnerMessageReturned()
        {
            var telemetry = Substitute.For<ITelemetry>();
            var reporter = new TelemetryBenchmarkResultAnalysisReporter(telemetry);

            var msgs = Enumerable.Range(1, 3).Select(_ => Guid.NewGuid().ToString()).ToList();
            var innerResults = msgs.Select(msg => new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
                Message = msg
            }).ToList();

            var analysis = new BenchmarkResultAnalysis()
            {
                MeetsRequirements = false,
                Message = "test message",
                InnerResults = innerResults,
            };

            var result = reporter.Report(analysis);

            result.Should().BeFalse();
            telemetry.Received(0).Success(Arg.Any<string>());
            telemetry.Received(1).Error(Arg.Is<string>(s => msgs.Any(s.Contains)));
            telemetry.Received(1).Error(Arg.Is<string>(s => s.Contains(analysis.Message)));
        }

    }
}

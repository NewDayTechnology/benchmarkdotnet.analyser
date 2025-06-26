using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using Shouldly;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Analysis
{
    public class BaselineDevianceAnalyserTests
    {
        [Property(Verbose = true)]
        public bool CreateAnalysis_TestValuesWithinTolerance(PositiveInt meanValue, PositiveInt toleranceValue)
        {
            var baselineValue = meanValue.Get;
            var tolerance = toleranceValue.Get / 100.0m;
            var testValue = (meanValue.Get * (1 + tolerance));

            var baseline = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = baselineValue});
            var test = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = testValue});
            
            var analyser = new BaselineDevianceAnalyser("MeanTime", br => br.MeanTime!.Value, tolerance);

            var name = "abc";

            var result = analyser.CreateAnalysis(name, baseline, test);

            return result.MeetsRequirements &&
                   result.Message == null &&
                   result.BenchmarkName == name;
        }

        [Property(Verbose = true)]
        public bool CreateAnalysis_TestValuesOutsideOfTolerance(PositiveInt meanValue, PositiveInt toleranceValue)
        {
            var baselineValue = meanValue.Get;
            var tolerance = toleranceValue.Get / 100.0m;
            var testValue = (meanValue.Get * (1 + tolerance)) + 1;
            var statistic = "MeanTime";

            var baseline = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = baselineValue});
            var test = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = testValue});
            
            var analyser = new BaselineDevianceAnalyser(statistic, br => br.MeanTime!.Value, tolerance);

            var name = "abc";
            var result = analyser.CreateAnalysis(name, baseline, test);

            return !result.MeetsRequirements &&
                !string.IsNullOrWhiteSpace(result.Message) &&
                result.Message.Contains(statistic) &&
                result.BenchmarkName == name;
        }

        
        [Theory]
        [InlineData(1, -1, 0.9)]
        [InlineData(1, 2, 0.9)]
        [InlineData(1, 0, 0.9)]
        [InlineData(-1, 1, 0)]
        [InlineData(-1, 1, 0.1)]
        public void CreateAnalysis_ToleranceHasPositiveNegativeBoundaries_OutsideBoundaries(decimal baselineValue, decimal testValue, decimal tolerance)
        {
            var baseline = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = baselineValue});
            var test = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = testValue});
            var statistic = "MeanTime";

            var analyser = new BaselineDevianceAnalyser(statistic, br => br.MeanTime!.Value, tolerance);
            
            var result = analyser.CreateAnalysis("", baseline, test);

            result.MeetsRequirements.ShouldBeFalse();
        }

        [Theory]
        [InlineData(1, 1.9, 0.9)]
        [InlineData(1, 0.1, 0.9)]
        public void CreateAnalysis_ToleranceHasPositiveNegativeBoundaries_InsideBoundaries(decimal baselineValue, decimal testValue, decimal tolerance)
        {
            var baseline = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = baselineValue});
            var test = (new BenchmarkRunInfo(), new BenchmarkResult() {MeanTime = testValue});
            var statistic = "MeanTime";

            var analyser = new BaselineDevianceAnalyser(statistic, br => br.MeanTime!.Value, tolerance);
            
            var result = analyser.CreateAnalysis("", baseline, test);

            result.MeetsRequirements.ShouldBeTrue();
        }
    }
}

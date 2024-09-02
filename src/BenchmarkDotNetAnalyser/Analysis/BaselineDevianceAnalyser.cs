using System;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class BaselineDevianceAnalyser
    {
        private readonly Func<BenchmarkResult, decimal> _getResultValue;
        private readonly string _statistic;
        private readonly decimal _devianceTolerance;

        public BaselineDevianceAnalyser(string statistic, Func<BenchmarkResult, decimal> getResultValue, decimal devianceTolerance)
        {
            _getResultValue = getResultValue.ArgNotNull(nameof(getResultValue));
            _statistic = statistic;
            _devianceTolerance = devianceTolerance;
        }

        public BenchmarkResultAnalysis CreateAnalysis(string name,
            (BenchmarkRunInfo, BenchmarkResult) baseline, 
            (BenchmarkRunInfo, BenchmarkResult) test)
        {
            var baselineValue = _getResultValue(baseline.Item2);
            var testValue = _getResultValue(test.Item2);

            var e = baselineValue * _devianceTolerance;
            var lower = baselineValue - e;
            var upper = baselineValue + e;
            
            var withinTolerance = testValue >= lower && testValue <= upper;
            string message = null;

            if (!withinTolerance)
            {
                message = $"Benchmark {name} does not meet requirements.{Environment.NewLine}Baseline {_statistic}: {baselineValue}\ttolerance: +/- {e}\tActual: {testValue}";
            }

            return new BenchmarkResultAnalysis()
            {
                BenchmarkName = name,
                Message = message,
                MeetsRequirements = withinTolerance,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class BaselineDevianceGroupAnalyser
    {
        private readonly ITelemetry _telemetry;
        private readonly decimal _deviance;

        public BaselineDevianceGroupAnalyser(ITelemetry telemetry, decimal deviance)
        {
            _telemetry = telemetry;
            _deviance = deviance;
        }

        public IEnumerable<BenchmarkResultAnalysis> Analyse(BenchmarkResultGroup group)
        {
            group.ArgNotNull(nameof(group));
            var items = group.Results.NullToEmpty()
                            .OrderByDescending(a => a.Item1.Creation)
                            .ToList();
            if (items.Count < 2)
            {
                var name = items.Count == 1 ? items[0].Item2.FullName : null;
                yield return new BenchmarkResultAnalysis()
                {
                    MeetsRequirements = true,
                    BenchmarkName = name,
                };
            }
            else
            {
                Func<BenchmarkResult, decimal> getResultValue = r => r.Mean.GetValueOrDefault();

                var baseline = items.MinBy(t => getResultValue(t.Item2));
                var test = items[0];

                var analysis = new BaselineDevianceAnalyser(getResultValue, _deviance).CreateAnalysis(group.Name, baseline, test);

                if (!analysis.MeetsRequirements)
                {
                    TelemetryEntry.Error(analysis.Message, true, true).PipeDo(_telemetry.Write);
                }

                yield return analysis;
            }
        }
    }
}

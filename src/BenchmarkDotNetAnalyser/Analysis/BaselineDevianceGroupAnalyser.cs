using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class BaselineDevianceGroupAnalyser
    {
        private readonly ITelemetry _telemetry;
        private readonly IBenchmarkStatisticAccessorProvider _accessors;
        private readonly decimal _deviance;
        private readonly string _statistic;

        public BaselineDevianceGroupAnalyser(ITelemetry telemetry, IBenchmarkStatisticAccessorProvider accessors, decimal deviance, string statistic)
        {
            _telemetry = telemetry;
            _accessors = accessors;
            _deviance = deviance;
            _statistic = statistic;
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
                var getResultValue = _accessors.GetAccessor(_statistic);
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

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class BaselineDevianceBenchmarkAnalyser : BaseBenchmarkAnalyser
    {
        private readonly IBenchmarkStatisticAccessorProvider _accessors;
        private readonly decimal _deviance;
        private readonly int _maxErrors;
        private readonly string _statistic;

        public BaselineDevianceBenchmarkAnalyser(ITelemetry telemetry, IBenchmarkStatisticAccessorProvider accessors, string basePath, decimal deviance, int maxErrors, string statistic) 
            : base(telemetry, basePath)
        {
            _accessors = accessors;
            _deviance = deviance;
            _maxErrors = maxErrors;
            _statistic = statistic;
        }

        protected override IEnumerable<BenchmarkResultAnalysis> AnalyseGroups(IEnumerable<BenchmarkResultGroup> benchmarkResultGroups)
        {
            var groupAnalyser = new BaselineDevianceGroupAnalyser(Telemetry, _accessors, _deviance, _statistic);

            IEnumerable<BenchmarkResultAnalysis> analyseGroup(BenchmarkResultGroup grp) =>
                Telemetry.InvokeWithLogging(TelemetryEntry.Commentary($"Analysing benchmarks for {grp.Name}..."), () => groupAnalyser.Analyse(grp));

            return benchmarkResultGroups.SelectMany(analyseGroup);
        }

        protected override BenchmarkResultAnalysis Consolidate(IEnumerable<BenchmarkResultAnalysis> values) =>
            new BenchmarkResultAnalysisAggregator(_maxErrors).Consolidate(values);
    }
}

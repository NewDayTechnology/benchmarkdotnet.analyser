using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class BaselineDevianceBenchmarkAnalyser : BaseBenchmarkAnalyser
    {
        private readonly decimal _deviance;
        private readonly int _maxErrors;

        public BaselineDevianceBenchmarkAnalyser(ITelemetry telemetry, string basePath, decimal deviance, int maxErrors) 
            : base(telemetry, basePath)
        {
            _deviance = deviance;
            _maxErrors = maxErrors;
        }

        protected override IEnumerable<BenchmarkResultAnalysis> AnalyseGroups(IEnumerable<BenchmarkResultGroup> benchmarkResultGroups)
        {
            var groupAnalyser = new BaselineDevianceGroupAnalyser(Telemetry, _deviance);

            IEnumerable<BenchmarkResultAnalysis> analyseGroup(BenchmarkResultGroup grp) =>
                Telemetry.InvokeWithLogging(TelemetryEntry.Commentary($"Analysing benchmarks for {grp.Name}..."), () => groupAnalyser.Analyse(grp));

            return benchmarkResultGroups.SelectMany(analyseGroup);
        }

        protected override BenchmarkResultAnalysis Consolidate(IEnumerable<BenchmarkResultAnalysis> values) =>
            new BenchmarkResultAnalysisAggregator(_maxErrors).Consolidate(values);
    }
}

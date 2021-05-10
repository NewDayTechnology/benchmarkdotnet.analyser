using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public abstract class BaseBenchmarkAnalyser : IBenchmarkAnalyser
    {
        protected readonly ITelemetry Telemetry;
        private readonly BenchmarkRunResultsReader _runResultsReader;
        
        protected BaseBenchmarkAnalyser(ITelemetry telemetry)
        {
            Telemetry = telemetry.ArgNotNull(nameof(telemetry));
            _runResultsReader = new BenchmarkRunResultsReader();
            
        }

        public Task<BenchmarkResultAnalysis> AnalyseAsync(IEnumerable<BenchmarkInfo> benchmarks)
        {
            var benchmarkResults = Telemetry.InvokeWithLogging(TelemetryEntry.Commentary("Reading benchmark results..."), () => _runResultsReader.GetBenchmarkResults(benchmarks.NullToEmpty()));
            var result = new BenchmarkResultGroupBuilder()
                                .FromResults(benchmarkResults)
                                .Pipe(AnalyseGroups)
                                .Pipe(Consolidate);

            return Task.FromResult(result);
        }

        protected virtual BenchmarkResultAnalysis Consolidate(IEnumerable<BenchmarkResultAnalysis> values) =>
            new BenchmarkResultAnalysisAggregator(0).Consolidate(values);
        

        protected abstract IEnumerable<BenchmarkResultAnalysis> AnalyseGroups(IEnumerable<BenchmarkResultGroup> benchmarkResultGroups);
    }
}

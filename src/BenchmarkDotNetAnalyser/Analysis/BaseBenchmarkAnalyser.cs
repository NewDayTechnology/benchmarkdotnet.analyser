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
        
        protected BaseBenchmarkAnalyser(ITelemetry telemetry, string basePath)
            : this(telemetry, new BenchmarkResultJsonFileReader(), basePath)
        {
        }

        protected BaseBenchmarkAnalyser(ITelemetry telemetry, IBenchmarkResultReader resultReader, string basePath)
        {
            Telemetry = telemetry.ArgNotNull(nameof(telemetry));
            _runResultsReader = new BenchmarkRunResultsReader(resultReader, basePath);
            
        }

        public async Task<BenchmarkResultAnalysis> AnalyseAsync(IEnumerable<BenchmarkInfo> benchmarks)
        {
            var benchmarkResults = await Telemetry.InvokeWithLoggingAsync(TelemetryEntry.Commentary("Reading benchmark results..."), () => _runResultsReader.GetBenchmarkResults(benchmarks.NullToEmpty()));
            
            return new BenchmarkResultGroupBuilder()
                                .FromResults(benchmarkResults)
                                .Pipe(AnalyseGroups)
                                .Pipe(Consolidate);
        }

        protected virtual BenchmarkResultAnalysis Consolidate(IEnumerable<BenchmarkResultAnalysis> values) =>
            new BenchmarkResultAnalysisAggregator(0).Consolidate(values);
        

        protected abstract IEnumerable<BenchmarkResultAnalysis> AnalyseGroups(IEnumerable<BenchmarkResultGroup> benchmarkResultGroups);
    }
}

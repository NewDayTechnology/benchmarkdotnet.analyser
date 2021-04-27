using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;

namespace BenchmarkDotNetAnalyser.Commands
{
    public class AnalyseBenchmarksExecutor : IAnalyseBenchmarksExecutor
    {
        private readonly ITelemetry _telemetry;
        private readonly IBenchmarkInfoProvider _infoProvider;
        private readonly Func<AnalyseBenchmarksExecutorArgs, IBenchmarkAnalyser> _getAnalyser;
        
        public AnalyseBenchmarksExecutor(ITelemetry telemetry, IBenchmarkInfoProvider infoProvider)
        {
            _telemetry = telemetry;
            _infoProvider = infoProvider;
            _getAnalyser = CreateAnalyser;
        }

        internal AnalyseBenchmarksExecutor(ITelemetry telemetry, IBenchmarkInfoProvider infoProvider,
                                            Func<AnalyseBenchmarksExecutorArgs, IBenchmarkAnalyser> getAnalyser)
            : this(telemetry, infoProvider)
        {
            _getAnalyser = getAnalyser;
        }

        public async Task<bool> ExecuteAsync(AnalyseBenchmarksExecutorArgs args)
        {
            args.ArgNotNull(nameof(args));
            
            var benchmarks = await _telemetry.InvokeWithLoggingAsync(TelemetryEntry.Commentary("Getting benchmarks..."), 
                                                                    () => GetAggregateBenchmarksAsync(args.AggregatesPath));
            if (benchmarks.Count == 0)
            {
                _telemetry.Warning("No benchmarks found.");
                return false;
            }
            _telemetry.Commentary($"{benchmarks.Count} benchmark(s) found.");
            
            _telemetry.Info("Analysing benchmarks...");
            var result = await _getAnalyser(args).AnalyseAsync(benchmarks);
            _telemetry.Info("Analysis done.");

            return ReportAnalysis(_telemetry, result);
        }

        protected async Task<IList<BenchmarkInfo>> GetAggregateBenchmarksAsync(string aggregatesPath)
        {
            var infos = await _infoProvider.GetBenchmarkInfosAsync(aggregatesPath);

            return infos
                .NullToEmpty()
                .Where(bi => bi != null)
                .ToList();
        }

        private bool ReportAnalysis(ITelemetry telemetry, BenchmarkResultAnalysis result)
        {
            var reporter = new TelemetryBenchmarkResultAnalysisReporter(telemetry);

            return reporter.Report(result);
        }

        private IBenchmarkAnalyser CreateAnalyser(AnalyseBenchmarksExecutorArgs args) => 
            new BaselineDevianceBenchmarkAnalyser(_telemetry, args.AggregatesPath, args.Tolerance, args.MaxErrors);
    }
}

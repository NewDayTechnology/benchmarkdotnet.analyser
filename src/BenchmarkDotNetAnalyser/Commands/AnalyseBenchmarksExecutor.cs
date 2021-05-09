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
        private readonly IBenchmarkStatisticAccessorProvider _accessors;
        private readonly Func<AnalyseBenchmarksExecutorArgs, IBenchmarkAnalyser> _getAnalyser;
        
        public AnalyseBenchmarksExecutor(ITelemetry telemetry, IBenchmarkInfoProvider infoProvider, IBenchmarkStatisticAccessorProvider accessors)
        {
            _telemetry = telemetry;
            _infoProvider = infoProvider;
            _accessors = accessors;
            _getAnalyser = CreateAnalyser;
        }

        internal AnalyseBenchmarksExecutor(ITelemetry telemetry, IBenchmarkInfoProvider infoProvider, IBenchmarkStatisticAccessorProvider accessors,
                                            Func<AnalyseBenchmarksExecutorArgs, IBenchmarkAnalyser> getAnalyser)
            : this(telemetry, infoProvider, accessors)
        {
            _getAnalyser = getAnalyser;
        }

        public async Task<BenchmarkResultAnalysis> ExecuteAsync(AnalyseBenchmarksExecutorArgs args)
        {
            args.ArgNotNull(nameof(args));

            (new TelemetryAnalyseBenchmarksExecutorArgsReporter(_telemetry)).Report(args);
            
            var benchmarks = await _telemetry.InvokeWithLoggingAsync(TelemetryEntry.Commentary("Getting benchmarks..."), 
                                                                    () => GetAggregateBenchmarksAsync(args.AggregatesPath));
            if (benchmarks.Count == 0)
            {
                var msg = "No benchmarks found.";
                _telemetry.Error(msg);
                return new BenchmarkResultAnalysis()
                {
                    MeetsRequirements = false,
                    Message = msg,
                };
            }
            _telemetry.Commentary($"{benchmarks.Count} benchmark(s) found.");
            
            _telemetry.Info("Analysing benchmarks...");
            var result = await _getAnalyser(args).AnalyseAsync(benchmarks);
            _telemetry.Info("Analysis done.");

            return result;
            
        }

        protected async Task<IList<BenchmarkInfo>> GetAggregateBenchmarksAsync(string aggregatesPath)
        {
            var infos = await _infoProvider.GetBenchmarkInfosAsync(aggregatesPath);

            return infos
                .NullToEmpty()
                .Where(bi => bi != null)
                .ToList();
        }


        private IBenchmarkAnalyser CreateAnalyser(AnalyseBenchmarksExecutorArgs args) => 
            new BaselineDevianceBenchmarkAnalyser(_telemetry, _accessors, args.AggregatesPath, args.Tolerance, args.MaxErrors, args.Statistic);
    }
}

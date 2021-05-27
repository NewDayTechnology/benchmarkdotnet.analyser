using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Aggregation;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Instrumentation;
using BenchmarkDotNetAnalyser.IO;

namespace BenchmarkDotNetAnalyser.Commands
{
    public class AggregateBenchmarksExecutor : IAggregateBenchmarksExecutor
    {
        private readonly ITelemetry _telemetry;
        private readonly IFileFinder _fileFinder;
        private readonly IBenchmarkRunInfoProvider _runInfoProvider;
        private readonly IBenchmarkInfoProvider _infoProvider;
        private readonly IBenchmarkAggregator _benchmarkAggregator;
        private AggregateBenchmarksExecutorArgs _args;

        public AggregateBenchmarksExecutor(ITelemetry telemetry,
            IFileFinder fileFinder, 
            IBenchmarkRunInfoProvider runInfoProvider,
            IBenchmarkInfoProvider infoProvider, 
            IBenchmarkAggregator benchmarkAggregator)
        {
            _telemetry = telemetry;
            _fileFinder = fileFinder;
            _runInfoProvider = runInfoProvider;
            _infoProvider = infoProvider;
            _benchmarkAggregator = benchmarkAggregator;
        }

        public async Task<bool> ExecuteAsync(AggregateBenchmarksExecutorArgs args)
        {
            _args = args.ArgNotNull(nameof(args));

            (new TelemetryAggregateBenchmarksExecutorArgsReporter(_telemetry)).Report(args);

            var newBenchmark = await _telemetry.InvokeWithLoggingAsync(TelemetryEntry.Commentary("Getting new benchmark..."), 
                                                                        GetNewBenchmarkAsync);
            
            var existingBenchmarks = await _telemetry.InvokeWithLoggingAsync(TelemetryEntry.Commentary("Getting prior benchmarks..."), 
                                                                                GetAggregateBenchmarksAsync);
            
            var aggregation = _telemetry.InvokeWithLogging(TelemetryEntry.Commentary("Aggregating..."), 
                                                            () => Aggregate(existingBenchmarks, newBenchmark));
            
            var writeResult = await _telemetry.InvokeWithLoggingAsync(TelemetryEntry.Commentary("Writing aggregations..."), 
                                                                        () => WriteAggregateBenchmarksAsync(aggregation));

            if (writeResult) _telemetry.Success("Aggregation complete.");
            
            return writeResult;
        }

        
        private IEnumerable<BenchmarkInfo> Aggregate(IList<BenchmarkInfo> existingBenchmarks, BenchmarkInfo newBenchmark)
        {
            var opts = new BenchmarkAggregationOptions()
            {
                PreservePinned = true,
                Runs = _args.BenchmarkRuns,
            };

            return _benchmarkAggregator.Aggregate(opts, existingBenchmarks, newBenchmark).ToList();
        }


        private async Task<BenchmarkInfo> GetNewBenchmarkAsync()
        {
            var fetches = _fileFinder
                .Find(_args.NewBenchmarksPath, _args.DataFileSuffix)
                .Select(_runInfoProvider.GetRunInfoAsync)
                .ToArray();

            var newFiles = (await Task.WhenAll(fetches))
                .Where(bri => bri != null)
                .ToList();

            return new BenchmarkInfo()
            {
                Creation = newFiles.Any()
                    ? newFiles.Select(bri => bri.Creation).Min()
                    : default,
                Tags = _args.Tags,
                BuildNumber = _args.BuildNumber,
                BuildUri = _args.BuildUri,
                BranchName = _args.BranchName,
                CommitSha = _args.CommitSha,
                Runs = newFiles,
            };
        }

        private async Task<IList<BenchmarkInfo>> GetAggregateBenchmarksAsync()
        {
            var infos = await _infoProvider.GetBenchmarkInfosAsync(_args.AggregatedBenchmarksPath);

            return infos
                .NullToEmpty()
                .Where(bi => bi != null)
                .ToList();
        }

        private async Task<bool> WriteAggregateBenchmarksAsync(IEnumerable<BenchmarkInfo> values)
        {
            await _infoProvider.WriteBenchmarkInfosAsync(_args.OutputAggregatesPath, values);

            return true;
        }

    }
}

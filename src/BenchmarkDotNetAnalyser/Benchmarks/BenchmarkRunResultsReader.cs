using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    internal class BenchmarkRunResultsReader : IBenchmarkRunResultsReader
    {
        private readonly IBenchmarkResultReader _resultReader;
        private readonly string _basePath;

        public BenchmarkRunResultsReader(IBenchmarkResultReader resultReader, string basePath)
        {
            _resultReader = resultReader.ArgNotNull(nameof(resultReader));
            _basePath = basePath.ArgNotNull(nameof(basePath));
        }

        public async Task<IList<BenchmarkRunResults>> GetBenchmarkResults(IEnumerable<BenchmarkInfo> benchmarks)
        {
            var result = new List<BenchmarkRunResults>();

            foreach (var benchmarkInfo in benchmarks)
            {
                var xs = await GetBenchmarkResults(benchmarkInfo);
                result.AddRange(xs);
            }

            return result;
        }

        private async Task<IList<BenchmarkRunResults>> GetBenchmarkResults(BenchmarkInfo benchmark)
        {
            var result = new List<BenchmarkRunResults>();

            foreach (var bri in benchmark.Runs)
            {
                var path = Path.Combine(_basePath, bri.FullPath);

                var r = new BenchmarkRunResults()
                {
                    Run = bri,
                    Results = (await _resultReader.GetBenchmarkResultsAsync(path)) ?? new List<BenchmarkResult>(),
                };

                result.Add(r);
            }

            return result;
        }

    }
}

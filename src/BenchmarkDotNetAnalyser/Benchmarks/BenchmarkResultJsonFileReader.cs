using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkResultJsonFileReader : IBenchmarkResultReader
    {
        public async Task<IList<BenchmarkResult>> GetBenchmarkResultsAsync(string path)
        {
            var json = await BenchmarkJsonFileReader.ReadJson(path);
            if (json == null)
            {
                return null;
            }

            return ParseBenchmarkResults(json).ToList();
        }
        
        private IEnumerable<BenchmarkResult> ParseBenchmarkResults(string json) => new BenchmarkParser(json).GetBenchmarkResults();
    }
}

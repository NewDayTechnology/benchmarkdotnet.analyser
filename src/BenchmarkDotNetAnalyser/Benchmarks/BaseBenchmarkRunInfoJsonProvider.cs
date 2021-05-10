using System.Linq;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public abstract class BaseBenchmarkRunInfoJsonProvider : IBenchmarkRunInfoProvider
    {
        public async Task<BenchmarkRunInfo> GetRunInfoAsync(string path)
        {
            path.ArgNotNull(nameof(path));

            var json = await GetBenchmarkJsonAsync(path);
            if (json == null) return null;

            var parser = new BenchmarkParser(json);
            var env = parser.GetBenchmarkEnvironment();
            
            return new BenchmarkRunInfo()
            {
                Creation = parser.GetCreation(),
                BenchmarkDotNetVersion = env.BenchmarkDotNetVersion,
                Results = parser.GetBenchmarkResults().ToList()
            };

        }

        protected abstract Task<string> GetBenchmarkJsonAsync(string path);
    }
}

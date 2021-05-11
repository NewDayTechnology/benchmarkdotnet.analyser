using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public abstract class BaseBenchmarkInfoJsonProvider : IBenchmarkInfoProvider
    {
        public async Task<IList<BenchmarkInfo>> GetBenchmarkInfosAsync(string path)
        {
            path.ArgNotNull(nameof(path));
            
            var json = await GetBenchmarkDataJsonAsync(path);
            
            return json != null
                ? Parse(json)
                : null;
        }

        public async Task<string> WriteBenchmarkInfosAsync(string destinationPath, IEnumerable<BenchmarkInfo> values)
        {
            destinationPath.ArgNotNull(nameof(destinationPath));
            values = values.ArgNotNull(nameof(values)).ToList();
            
            var json = GetBenchmarkDataJson(values);
            
            return await WriteBenchmarkDataJsonAsync(destinationPath, json);
        }
        
        protected abstract Task<string> GetBenchmarkDataJsonAsync(string path);

        protected abstract Task<string> WriteBenchmarkDataJsonAsync(string path, string json);
        
        private string GetBenchmarkDataJson(IEnumerable<BenchmarkInfo> values)
        {
            var xs = values.ToArray();

            return JsonConvert.SerializeObject(xs, Formatting.Indented);
        }
        
        private IList<BenchmarkInfo> Parse(string json) =>
            JsonConvert.DeserializeObject<IList<BenchmarkInfo>>(json);

        
    }
}

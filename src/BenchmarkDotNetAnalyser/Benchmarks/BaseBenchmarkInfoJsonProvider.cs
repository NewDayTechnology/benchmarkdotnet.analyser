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
            
            var json = await GetBenchmarkManifestJsonAsync(path);
            
            return json != null
                ? Parse(json)
                : null;
        }

        public async Task WriteBenchmarkInfosAsync(string sourcePath, string destinationPath, IEnumerable<BenchmarkInfo> values)
        {
            sourcePath.ArgNotNull(nameof(sourcePath));
            destinationPath.ArgNotNull(nameof(destinationPath));
            values = values.ArgNotNull(nameof(values)).ToList();

            await CopyBenchmarkFilesAsync(sourcePath, destinationPath, values);

            var json = GetBenchmarkManifestJson(values);
            
            await WriteBenchmarkManifestJsonAsync(destinationPath, json);
        }
        
        protected abstract Task<string> GetBenchmarkManifestJsonAsync(string path);

        protected abstract Task WriteBenchmarkManifestJsonAsync(string path, string json);

        protected abstract Task CopyBenchmarkFilesAsync(string sourcePath, string destinationPath,
            IEnumerable<BenchmarkInfo> benchmarkInfos);
        
        private string GetBenchmarkManifestJson(IEnumerable<BenchmarkInfo> values)
        {
            var xs = values.ToArray();

            return JsonConvert.SerializeObject(xs, Formatting.Indented);
        }
        
        private IList<BenchmarkInfo> Parse(string json) =>
            JsonConvert.DeserializeObject<IList<BenchmarkInfo>>(json);

        
    }
}

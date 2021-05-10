using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkInfoJsonFileProvider : BaseBenchmarkInfoJsonProvider
    {
        // TODO: rename - it's not a manifest!
        private const string ManifestFileName = "aggregatebenchmarks.manifest.json";

        protected override async Task<string> GetBenchmarkManifestJsonAsync(string path)
        {
            var filePath = GetBenchmarksManifestPath(path);

            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }
            return null;
        }

        protected override Task WriteBenchmarkManifestJsonAsync(string path, string json)
        {
            var filePath = GetBenchmarksManifestPath(path);

            return File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }
        
        private string GetBenchmarksManifestPath(string path) => Path.Combine(path, ManifestFileName);
    }
}

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkInfoJsonFileProvider : BaseBenchmarkInfoJsonProvider
    {
        private const string DataFileName = "aggregatebenchmarks.data.json";

        protected override async Task<string> GetBenchmarkDataJsonAsync(string path)
        {
            var filePath = GetBenchmarksDataPath(path);

            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }
            return null;
        }

        protected override async Task<string> WriteBenchmarkDataJsonAsync(string path, string json)
        {
            var filePath = GetBenchmarksDataPath(path);

            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);

            return filePath;
        }
        
        private string GetBenchmarksDataPath(string path) => Path.Combine(path, DataFileName);
    }
}

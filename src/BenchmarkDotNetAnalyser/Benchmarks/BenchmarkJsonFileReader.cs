using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public static class BenchmarkJsonFileReader
    {
        public static async Task<string> ReadJsonAsync(string path) => File.Exists(path) ? await File.ReadAllTextAsync(path) : null;
    }
}

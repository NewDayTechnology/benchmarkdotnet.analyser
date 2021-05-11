using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.IO;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkRunInfoJsonFileProvider : BaseBenchmarkRunInfoJsonProvider
    {
        protected override Task<string> GetBenchmarkJsonAsync(string path) => FileReader.ReadAsync(path);
    }
}

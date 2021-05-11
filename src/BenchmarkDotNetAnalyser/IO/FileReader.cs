using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.IO
{
    [ExcludeFromCodeCoverage]
    public static class FileReader
    {
        public static async Task<string> ReadAsync(string path) => File.Exists(path) ? await File.ReadAllTextAsync(path) : null;
    }
}

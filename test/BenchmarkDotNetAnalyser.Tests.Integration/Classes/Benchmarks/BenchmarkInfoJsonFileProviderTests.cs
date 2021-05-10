using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.Classes.Benchmarks
{
    public class BenchmarkInfoJsonFileProviderTests
    {
        public static IEnumerable<object[]> GetFilePaths() => IOHelper.GetSourceResultFilePaths().Select(x => new[] {x});

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task ReadWrite(string filePath)
        {
            var reader = new BenchmarkResultJsonFileReader();
            var result = await reader.GetBenchmarkResultsAsync(filePath);

            var p = new BenchmarkInfoJsonFileProvider();

            var xs = await p.GetBenchmarkInfosAsync(filePath);


            //p.WriteBenchmarkInfosAsync()
        }
    }
}

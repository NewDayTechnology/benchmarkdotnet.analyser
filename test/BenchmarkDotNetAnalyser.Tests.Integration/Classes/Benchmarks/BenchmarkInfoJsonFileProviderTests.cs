using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using FluentAssertions;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.Classes.Benchmarks
{
    public class BenchmarkInfoJsonFileProviderTests
    {
        public static IEnumerable<object[]> GetFilePaths() => IOHelper.GetSourceResultFilePaths().Select(x => new[] {x});

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetBenchmarkInfosAsync_WriteBenchmarkInfosAsync_Symmetric(string sourceFilePath)
        {
            var p = new BenchmarkInfoJsonFileProvider();
            
            
            var sourceReader = new BenchmarkRunInfoJsonFileProvider();
            var runInfo = await sourceReader.GetRunInfoAsync(sourceFilePath);
            var benchmarkInfo = new BenchmarkInfo()
            {
                Runs = new[] { runInfo },
            };
            var benchmarkInfos = benchmarkInfo.Singleton();
            var benchmarkInfosResultCount = benchmarkInfos.Sum(bi => bi.Runs.Sum(bri => bri.Results.Count));
            
            var workingDir = IOHelper.CreateTempFolder();
            var dir = IOHelper.CreateTempFolder(workingDir, nameof(BenchmarkInfoJsonFileProviderTests));
            
            var _ = await p.WriteBenchmarkInfosAsync(dir, benchmarkInfos);

            var readResult = await p.GetBenchmarkInfosAsync(dir);
            var readResultCount = readResult.Sum(r => r.Runs.Sum(x => x.Results.Count));
            
            benchmarkInfosResultCount.Should().Be(readResultCount);
        }
    }
}

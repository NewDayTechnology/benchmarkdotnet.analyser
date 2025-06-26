using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using Shouldly;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.Classes.Benchmarks
{
    public class BenchmarkRunInfoJsonFileProviderTests
    {
        public static IEnumerable<object[]> GetFilePaths() => IOHelper.GetSourceResultFilePaths().Select(x => new[] {x});

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetBenchmarkJsonAsync_ResultsParsed(string path)
        {
            var provider = new BenchmarkRunInfoJsonFileProvider();

            var result = await provider.GetRunInfoAsync(path);

            result.ShouldNotBeNull();
            result.BenchmarkDotNetVersion.ShouldNotBeNullOrWhiteSpace();
            result.Creation.ShouldNotBe(DateTimeOffset.MinValue);
            result.Results.Count.ShouldBeGreaterThan(0);
        }
    }
}

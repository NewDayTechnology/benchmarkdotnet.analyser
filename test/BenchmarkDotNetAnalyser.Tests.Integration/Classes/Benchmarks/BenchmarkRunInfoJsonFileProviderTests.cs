using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using FluentAssertions;
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

            result.Should().NotBeNull();
            result.FullPath.Should().Be(path);
            result.BenchmarkDotNetVersion.Should().NotBeNullOrWhiteSpace();
            result.Creation.Should().NotBe(DateTimeOffset.MinValue);
        }
    }
}

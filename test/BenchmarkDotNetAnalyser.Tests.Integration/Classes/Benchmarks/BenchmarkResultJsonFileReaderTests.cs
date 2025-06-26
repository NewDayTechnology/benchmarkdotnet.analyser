﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using Shouldly;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.Classes.Benchmarks
{
    public class BenchmarkResultJsonFileReaderTests
    {
        public static IEnumerable<object[]> GetFilePaths() => IOHelper.GetSourceResultFilePaths().Select(x => new[] {x});

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetBenchmarkResultsAsync_ResultsReturned(string path)
        {
            var reader = new BenchmarkResultJsonFileReader();

            var result = await reader.GetBenchmarkResultsAsync(path);

            result.Count.ShouldBeGreaterThan(0);
        }
    }
}

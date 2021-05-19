using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.IO;
using BenchmarkDotNetAnalyser.Reporting;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Reporting
{
    public class JsonBenchmarksReportGeneratorTest
    {
        [Fact]
        public async Task GenerateAsync_BenchmarksFlowThrough()
        {
            var benchmarks = new[] {new BenchmarkInfo()};
            var jsonWriter = Substitute.For<IJsonFileWriter>();
            var reader = Substitute.For<IBenchmarkReader>();
            reader.GetBenchmarkAsync(Arg.Any<string>(), Arg.Any<IList<string>>())
                .Returns(benchmarks);

            var gen = new JsonBenchmarksReportGenerator(jsonWriter, reader);

            ReportGenerationArgs args = new()
            {
                AggregatesPath = "testpath",
                OutputPath = "",
                Filters = null,
            };

            var results = await gen.GenerateAsync(args);

            results.Should().NotBeNull();
            await reader.Received(1)
                .GetBenchmarkAsync(Arg.Is<string>(s => s == args.AggregatesPath), Arg.Any<IList<string>>());
            await jsonWriter.Received(1).WriteAsync<object>(Arg.Any<IEnumerable<object>>(),
                Arg.Is<string>(s => s.EndsWith("benchmarks.json")));
        }
    }
}

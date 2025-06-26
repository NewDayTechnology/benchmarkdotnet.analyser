using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.IO;
using BenchmarkDotNetAnalyser.Reporting;
using Shouldly;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Reporting
{
    public class CsvBenchmarksReportGeneratorTest
    {
        [Fact]
        public async Task GenerateAsync_BenchmarksFlowThrough()
        {
            var benchmarks = new[] {new BenchmarkInfo() };
            var csvWriter = Substitute.For<ICsvFileWriter>();
            var reader = Substitute.For<IBenchmarkReader>();
            reader.GetBenchmarkAsync(Arg.Any<string>(), Arg.Any<IList<string>>())
                .Returns(benchmarks);

            var gen = new CsvBenchmarksReportGenerator(csvWriter, reader);

            ReportGenerationArgs args = new()
            {
                AggregatesPath = "testpath",
                OutputPath = "",
                Filters = null,
            };
            
            var results = await gen.GenerateAsync(args);

            results.ShouldNotBeNull();
            await reader.Received(1).GetBenchmarkAsync(Arg.Is<string>(s => s == args.AggregatesPath), Arg.Any<IList<string>>());
            csvWriter.Received(1).Write<object>(Arg.Any<IEnumerable<object>>(), Arg.Is<string>(s => s.EndsWith("benchmarks.csv")));
        }
    }
}

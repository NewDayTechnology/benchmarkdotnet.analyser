using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.IO;

namespace BenchmarkDotNetAnalyser.Reporting
{
    public class JsonBenchmarksReportGenerator : IBenchmarksReportGenerator
    {
        private readonly IJsonFileWriter _writer;
        private readonly IBenchmarkReader _benchmarkReader;

        public JsonBenchmarksReportGenerator(IJsonFileWriter writer, IBenchmarkReader benchmarkReader)
        {
            _writer = writer.ArgNotNull(nameof(writer));
            _benchmarkReader = benchmarkReader.ArgNotNull(nameof(benchmarkReader));
        }

        public async Task<IList<string>> GenerateAsync(ReportGenerationArgs args)
        {
            args.ArgNotNull(nameof(args));

            var benchmarks = await _benchmarkReader.GetBenchmarkAsync(args.AggregatesPath, args.Filters);
            var rows = benchmarks.ToBenchmarkRecords();

            var filePath = Path.Combine(args.OutputPath.ResolveWorkingPath(), "benchmarks.json");

            await _writer.WriteAsync(rows, filePath);

            return new[] { filePath };
        }
    }
}

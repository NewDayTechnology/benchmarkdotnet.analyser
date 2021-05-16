using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.IO;

namespace BenchmarkDotNetAnalyser.Reporting
{
    public class CsvBenchmarksReportGenerator : IBenchmarksReportGenerator
    {
        private readonly IBenchmarkReader _benchmarkReader;
        private readonly ICsvFileWriter _csvWriter;

        public CsvBenchmarksReportGenerator(ICsvFileWriter csvWriter, IBenchmarkReader benchmarkReader)
        {
            _csvWriter = csvWriter.ArgNotNull(nameof(csvWriter));
            _benchmarkReader = benchmarkReader.ArgNotNull(nameof(benchmarkReader));
        }

        public async Task<IList<string>> GenerateAsync(ReportGenerationArgs args)
        {
            args.ArgNotNull(nameof(args));

            var benchmarks = await _benchmarkReader.GetBenchmarkAsync(args.AggregatesPath, args.Filters);
            var rows = benchmarks.ToBenchmarkRecords().ToCsvRows();
            
            var filePath = Path.Combine(args.OutputPath.ResolveWorkingPath(), "benchmarks.csv");

            _csvWriter.Write(rows, filePath);
            
            return new[] { filePath };
        }

        
    }
}
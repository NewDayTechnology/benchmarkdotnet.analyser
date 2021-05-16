using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Reporting;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Reporting
{
    public class BenchmarkReaderTests
    {
        [Property(Verbose = true)]
        public bool GetBenchmarkAsync_GetCsv_NoFilterApplied(PositiveInt count)
        {
            var infos = Enumerable.Range(1, count.Get)
                .Select(i => new BenchmarkInfo()
                {
                    BranchName = i.ToString(),
                    Runs = new[] { new BenchmarkRunInfo() }
                }).ToList();
                
            var provider = Substitute.For<IBenchmarkInfoProvider>();
            provider.GetBenchmarkInfosAsync(Arg.Any<string>())
                .Returns(infos);

            var reader = new BenchmarkReader(provider);

            var args = new ReportGenerationArgs();

            var results = reader.GetBenchmarkAsync(args.AggregatesPath, args.Filters).Result;

            return results.SequenceEqual(infos);
        }

        [Property(Verbose = true)]
        public bool GetBenchmarkAsync_GetCsv_FilterApplied(PositiveInt count)
        {
            var infos = Enumerable.Range(1, count.Get)
                .Select(i => new BenchmarkInfo()
                {
                    BranchName = i.ToString(),
                    Runs = new[] { new BenchmarkRunInfo() }
                }).ToList();
                
            var provider = Substitute.For<IBenchmarkInfoProvider>();
            provider.GetBenchmarkInfosAsync(Arg.Any<string>())
                .Returns(infos);

            var reader = new BenchmarkReader(provider);

            var args = new ReportGenerationArgs()
            {
                Filters = new[] { Guid.NewGuid().ToString() }
            };

            var results = reader.GetBenchmarkAsync(args.AggregatesPath, args.Filters).Result.ToList();

            return results.Count == 0;
        }

    }
}

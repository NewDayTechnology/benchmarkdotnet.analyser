using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using FsCheck;
using FsCheck.Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Benchmarks
{
    public class BenchmarkRunResultsReaderTests
    {
        [Property(Verbose = true)]
        public bool GetBenchmarkResults_ResultsParsed(PositiveInt benchmarkCount, PositiveInt runCount, PositiveInt resultsCount)
        {
            var benchmarks = Enumerable.Range(1, benchmarkCount.Get)
                .Select(i => new BenchmarkInfo()
                {
                    BranchName = $"Branch{i}",
                    Runs = Enumerable.Range(1, runCount.Get)
                        .Select(j => new BenchmarkRunInfo()
                        {
                            BenchmarkDotNetVersion = $"Run {i} {j}",
                            Results = Enumerable.Range(1, resultsCount.Get)
                                .Select(k => new BenchmarkResult()
                                {
                                    FullName = $"Result {i} {j} {k}"
                                })
                                .ToList(),
                        })
                        .ToList(),
                }).ToList();
            var runs = benchmarks.SelectMany(bi => bi.Runs);
            var runResults = runs.SelectMany(bri => bri.Results);

            var rdr = new BenchmarkRunResultsReader();

            var result = rdr.GetBenchmarkResults(benchmarks);

            var actualRuns = result.Select(brr => brr.Run);
            var actualRunResults = actualRuns.SelectMany(bri => bri.Results);
            
            return result.Count == (runCount.Get * benchmarkCount.Get) &&
                runs.SequenceEqual(actualRuns) &&
                runResults.SequenceEqual(actualRunResults);
        }
    }
}

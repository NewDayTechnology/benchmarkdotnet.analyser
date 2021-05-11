using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Benchmarks
{
    public class BenchmarkInfoExtensionsTests
    {
        
        [Property(Verbose = true)]
        public bool PreservePinned_UnpinnedTrimmed(PositiveInt count)
        {
            var benchmarksCount = count.Get * 2;
            var benchmarks = Enumerable.Range(1, benchmarksCount)
                .Select(i => new BenchmarkInfo() {BranchName = $"Benchmark{i + 1}", Pinned = false})
                .ToList();
            var expected = benchmarks.Take(count.Get).ToList();

            var result = benchmarks.PreservePinned(count.Get).ToList();

            return result.SequenceEqual(expected);
        }

        [Property(Verbose = true)]
        public bool PreservePinned_InterpolatedPins_UnpinnedTrimmed(PositiveInt count)
        {
            var benchmarksCount = count.Get * 2; // ensure it's an even number
            
            // preserve even indexed
            var benchmarks = Enumerable.Range(1, benchmarksCount)
                .Select(i => new BenchmarkInfo() {BranchName = $"Benchmark{i + 1}", Pinned = i % 2 == 0})
                .ToList();
            
            var expected = benchmarks.Where(x => x.Pinned).ToList();

            var result = benchmarks.PreservePinned(count.Get).ToList();

            return result.SequenceEqual(expected);
        }

        [Theory]
        [InlineData(8,  new[] { true, true, true, true, true, true, true, true, true, true}, 
                        new[] { true, true, true, true,  true, true, true, true, true, true})]
        [InlineData(2,  new[] { true, true, true, true, true, true, true, true, true, true}, 
                        new[] { true,  true,  true, true,  true, true,  true, true,  true, true})]
        [InlineData(8,  new[] { false, false, true, false, true, false, true, false, true, true}, 
                        new[] { true,  true,  true, true,  true, false, true, false, true, true})]
        [InlineData(6,  new[] { false, false, true, false, true, false, true, false, true, true}, 
                        new[] { true,  false, true, false, true, false, true, false, true, true})]
        [InlineData(4,  new[] { false, false, true, false, true, false, true, false, true, true}, 
                        new[] { false, false, true, false, true, false, true, false, true, true})]
        [InlineData(2,  new[] { false, false, false, false, false, false, false, false, true, true}, 
                        new[] { false, false, false, false, false, false, false, false, true, true})]
        [InlineData(2,  new[] { true, true, false, false, false, false, false, false, false, false}, 
                        new[] { true, true, false, false, false, false, false, false, false, false})]
        [InlineData(2,  new[] { false, false, false, false, false, false, false, false, false, true}, 
                        new[] { true,  false, false, false, false, false, false, false, false, true})]
        public void PreservePinned_InterpolatedPinBitmap_BenchmarksTrimmed(int runs, bool[] pinned, bool[] survivors)
        {
            var benchmarks = pinned
                .Select((p,i) => new BenchmarkInfo() {BranchName = $"Benchmark{i + 1}", Pinned = p})
                .ToList();
            
            var result = benchmarks.PreservePinned(runs).ToList();

            var actual = benchmarks.Select(b => result.Contains(b)).ToArray();
            
            actual.SequenceEqual(survivors).Should().BeTrue();
        }

        [Fact]
        public void PinBest_SameValues_AllPinned()
        {
            var benchmarkResults = CreateResults("a", 3, (1.0M).Singleton().ToInfinity()).ToList();
            var benchmarks = CreateFromResults(benchmarkResults).Reverse().ToList();
            
            var stats = CreateMockStatsProvider();
            
            var _ = benchmarks.PinBest(stats);

            benchmarks.All(b => b.Pinned).Should().BeTrue();
        }

        [Fact]
        public void PinBest_DistinctValues_LowestPinned()
        {
            var benchmarkResults = CreateResults("a", 3).ToList();
            var benchmarks = CreateFromResults(benchmarkResults).Reverse().ToList();
            
            var stats = CreateMockStatsProvider();
            
            var _ = benchmarks.PinBest(stats);

            var lowestResult = benchmarkResults.MinBy(br => br.MeanTime.GetValueOrDefault());
            var lowest = benchmarks.Single(b => b.Runs.Any(r => r.Results.Contains(lowestResult)));
            var others = benchmarks.Where(b => b != lowest);

            lowest.Pinned.Should().BeTrue();
            others.All(b => !b.Pinned).Should().BeTrue();
        }

        [Fact]
        public void PinBest_DistinctValues_LowestPinned_MultipleRuns()
        {
            var benchmarkResults1 = CreateResults("a", 3).ToList();
            var benchmarkResults2 = CreateResults("b", 3).ToList();
            var benchmarkResults = benchmarkResults1.Concat(benchmarkResults2);


            var benchmarks = CreateFromResultGroups(benchmarkResults).Reverse().ToList();
            
            var stats = CreateMockStatsProvider();
            
            var _ = benchmarks.PinBest(stats);

            var lowestResult = benchmarkResults1.MinBy(br => br.MeanTime.GetValueOrDefault());
            var lowest = benchmarks.Single(b => b.Runs.Any(r => r.Results.Contains(lowestResult)));
            var others = benchmarks.Where(b => b != lowest);

            lowest.Pinned.Should().BeTrue();
            others.All(b => !b.Pinned).Should().BeTrue();
        }


        private IBenchmarkStatisticAccessorProvider CreateMockStatsProvider()
        {
            var result = Substitute.For<IBenchmarkStatisticAccessorProvider>();

            result.GetNullableAccessor(nameof(BenchmarkResult.MeanTime))
                .Returns(br => br.MeanTime);

            result.GetNullableAccessor(nameof(BenchmarkResult.MaxTime))
                .Returns(br => br.MaxTime);

            result.GetNullableAccessor(nameof(BenchmarkResult.MinTime))
                .Returns(br => br.MinTime);

            result.GetAccessorInfos()
                .Returns(new[] { nameof(BenchmarkResult.MeanTime), 
                        nameof(BenchmarkResult.MaxTime),
                        nameof(BenchmarkResult.MinTime)
                    }
                    .Select(n => new BenchmarkStatisticAccessorInfo(n, false)));

            return result;
        }

        private IEnumerable<BenchmarkResult> CreateResults(string name, int count, IEnumerable<decimal> values = null)
        {
            values ??= Enumerable.Range(1, count).Select(i => (decimal)i);

            var valuePairs = values.Select(i => new decimal[] {i, i * 10});

            return Enumerable.Range(1, count)
                .Zip(valuePairs, (i,xs) => (i,xs))
                .Select(t => new BenchmarkResult()
                {
                    FullName = name,
                    MeanTime = (decimal) t.xs[0],
                    MaxTime = (decimal) t.xs[1],
                });
        }

        private IEnumerable<BenchmarkInfo> CreateFromResults(IEnumerable<BenchmarkResult> results)
        {
            return results.Select(br => new BenchmarkRunInfo() {Results = new[] {br}})
                .Select((bri, i) => new BenchmarkInfo()
                {
                    BranchName = $"branch{i + 1}",
                    Runs = new[] {bri}
                });
        }

        private IEnumerable<BenchmarkInfo> CreateFromResultGroups(IEnumerable<BenchmarkResult> results)
        {
            var groups = results.GroupBy(br => br.FullName)
                .Select(grp => (grp.Key, grp.ToList()))
                .ToList();

            for (var x = 0; x < groups[0].Item2.Count; x++)
            {
                var results2 = groups.Select(grp => grp.Item2[x]).ToList();

                yield return new BenchmarkInfo()
                {
                    Runs = new[]
                    {
                        new BenchmarkRunInfo()
                        {
                            Results = results2,
                        }
                    }
                };
            }
        }
    }
}

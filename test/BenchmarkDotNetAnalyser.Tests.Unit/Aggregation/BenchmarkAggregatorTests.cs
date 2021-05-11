using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Aggregation;
using BenchmarkDotNetAnalyser.Benchmarks;
using FsCheck;
using FsCheck.Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Aggregation
{
    public class BenchmarkAggregatorTests
    {
        [Property(Verbose = true)]
        public bool Aggregate_BehavesAsAStack(PositiveInt existingCount, PositiveInt runs)
        {
            var newItem = new BenchmarkInfo() { BranchName = "0" };
            var existingItems = Enumerable.Range(1, existingCount.Get)
                                            .Select(x => new BenchmarkInfo() { BranchName = x.ToString()})
                                            .ToList();
            var opts = new BenchmarkAggregationOptions()
            {
                PreservePinned = false,
                Runs = Math.Min(Math.Max(runs.Get, existingCount.Get), existingItems.Count+1),
            };

            var result = new BenchmarkAggregator(new BenchmarkStatisticAccessorProvider()).Aggregate(opts, existingItems, newItem);

            var actual = result.Select(bi => int.Parse(bi.BranchName)).ToList();
            var expected = Enumerable.Range(0, opts.Runs);

            return expected.SequenceEqual(actual);
        }

        [Property(Verbose = true)]
        public bool Aggregate_PreservePinned_AllUnpinned_BehavesAsAStack(PositiveInt existingCount, PositiveInt runs)
        {
            var newItem = new BenchmarkInfo() { BranchName = "0" };
            var existingItems = Enumerable.Range(1, existingCount.Get)
                .Select(x => new BenchmarkInfo() { BranchName = x.ToString()})
                .ToList();
            var opts = new BenchmarkAggregationOptions()
            {
                PreservePinned = true,
                Runs = Math.Min(Math.Max(runs.Get, existingCount.Get), existingItems.Count+1),
            };

            var result = new BenchmarkAggregator(new BenchmarkStatisticAccessorProvider()).Aggregate(opts, existingItems, newItem);

            var actual = result.Select(bi => int.Parse(bi.BranchName)).ToList();
            var expected = Enumerable.Range(0, opts.Runs);

            return expected.SequenceEqual(actual);
        }

        [Property(Verbose = true)]
        public bool Aggregate_PreservePinned_SinglePinned_RiosingValues_PinnedPreserved(PositiveInt existingCount)
        {
            var newItem = new BenchmarkInfo() { BranchName = "0", Pinned = false };
            var existingItems = Enumerable.Range(1, existingCount.Get)
                .Select((x,i) => new BenchmarkInfo()
                {
                    BranchName = x.ToString(), Pinned = false,
                    Runs = new[] { new BenchmarkRunInfo() { Results = new[] { new BenchmarkResult() { MeanTime = (decimal)(existingCount.Get - i) }} }}
                })
                .ToList();
            var survivor = existingItems.Last();
            survivor.Pinned = true;
            var runs = existingCount.Get - 1;
            var opts = new BenchmarkAggregationOptions()
            {
                PreservePinned = true,
                Runs = Math.Max(runs, 1),
            };

            var result = new BenchmarkAggregator(new BenchmarkStatisticAccessorProvider()).Aggregate(opts, existingItems, newItem).ToList();
            
            return result.Contains(survivor);
        }

        [Property(Verbose = true)]
        public bool Aggregate_PreservePinned_EqualValues_PinnedPreserved(PositiveInt existingCount)
        {
            var newItem = new BenchmarkInfo() { BranchName = "0", Pinned = false };
            var existingItems = Enumerable.Range(1, existingCount.Get)
                .Select((x,i) => new BenchmarkInfo()
                {
                    BranchName = x.ToString(), Pinned = true,
                    Runs = new[] { new BenchmarkRunInfo()
                    {
                        Results = new[] { new BenchmarkResult() { MeanTime = 1.0M }}
                    } },
                })
                .ToList();
            var expected = newItem.Singleton().Concat(existingItems);

            var runs = existingCount.Get - 1;
            var opts = new BenchmarkAggregationOptions()
            {
                PreservePinned = true,
                Runs = Math.Max(runs, 1),
            };

            var result = new BenchmarkAggregator(new BenchmarkStatisticAccessorProvider()).Aggregate(opts, existingItems, newItem).ToList();

            return result.SequenceEqual(expected);
        }
    }
}

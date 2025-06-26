using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using Shouldly;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Analysis
{
    public class BenchmarkResultGroupBuilderTests
    {
        [Fact]
        public void FromResults_EmptyRuns_EmptyResults()
        {
            var runs = new List<BenchmarkRunResults>();

            var result = new BenchmarkResultGroupBuilder().FromResults(runs).ToList();

            result.Count.ShouldBe(0);
        }

        [Fact]
        public void FromResults_SingleRun_EmptyRun_EmptyResults()
        {
            var runs = new[]
            {
                new BenchmarkRunResults(),
            };

            var result = new BenchmarkResultGroupBuilder().FromResults(runs).ToList();

            result.Count.ShouldBe(0);
        }

        [Fact]
        public void FromResults_MultipleRuns_NamesMatch()
        {
            var names = new[] {"def", "abc"};
            var resultRuns = names.Select(n => new BenchmarkResult() {FullName = n}).ToList();

            var runs = new[]
            {
                new BenchmarkRunResults()
                {
                    Run = new BenchmarkRunInfo() { BenchmarkDotNetVersion = "a", Creation = DateTimeOffset.UtcNow },
                    Results = resultRuns,
                },
            };

            var result = new BenchmarkResultGroupBuilder().FromResults(runs).ToList();

            result.Count.ShouldBe(resultRuns.Count);
            result.Select(r => r.Name).SequenceEqual(names).ShouldBeTrue();
        }

        
        [Fact]
        public void FromResults_SingletonRun_ResultsInChronoOrder()
        {
            var fullName = "abc";
            var now = DateTime.UtcNow;
            var times = Enumerable.Range(1, 10).Reverse().Select(i => now.AddMinutes(-i)).ToList();

            var runs = times.Select(t =>
                new BenchmarkRunResults()
                {
                    Run = new BenchmarkRunInfo() {BenchmarkDotNetVersion = "a", Creation = t },
                    Results = new[] {new BenchmarkResult() {FullName = fullName,}},
                }
            ).ToList();
            
            var result = new BenchmarkResultGroupBuilder().FromResults(runs).ToList();

            result.Count.ShouldBe(1);

            var xs = result[0].Results.Select(r => r.Item1.Creation.DateTime).Reverse();
            xs.SequenceEqual(times).ShouldBe(true);
        }

        [Fact]
        public void FromResults_SingletonRun_ResultsMapped()
        {
            var now = DateTime.UtcNow;
            var times = Enumerable.Range(1, 10).Reverse().Select(i => now.AddMinutes(-i)).ToList();

            var runs = times.Select((t,i) =>
                new BenchmarkRunResults()
                {
                    Run = new BenchmarkRunInfo() {BenchmarkDotNetVersion = "a", Creation = t },
                    Results = new[] {new BenchmarkResult() {FullName = "abc", MinTime = i}},
                }
            ).ToList();
            var expectedValues = runs.SelectMany(rr => rr.Results).Select(r => (int) r.MinTime).Reverse();

            var result = new BenchmarkResultGroupBuilder().FromResults(runs).ToList();

            result.Count.ShouldBe(1);

            var xs = result.SelectMany(r => r.Results).Select(t => (int)t.Item2.MinTime);
            xs.SequenceEqual(expectedValues).ShouldBeTrue();
        }
    }
}

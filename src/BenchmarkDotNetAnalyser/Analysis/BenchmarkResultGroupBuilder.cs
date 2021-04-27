using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Analysis
{
    public class BenchmarkResultGroupBuilder
    {
        public IEnumerable<BenchmarkResultGroup> FromResults(IList<BenchmarkRunResults> runs) =>
            runs.NullToEmpty()
                .SelectMany(brr => brr.Results.NullToEmpty()
                                                .Select(br => new {Name = br.FullName, Result = br, Run = brr.Run}))
                .GroupBy(a => a.Name)
                .Select(grp => new BenchmarkResultGroup()
                {
                    Name = grp.Key,
                    Results = grp.OrderByDescending(a => a.Run.Creation)
                                 .Select(a => (a.Run, a.Result))
                                 .ToList()
                });

    }
}

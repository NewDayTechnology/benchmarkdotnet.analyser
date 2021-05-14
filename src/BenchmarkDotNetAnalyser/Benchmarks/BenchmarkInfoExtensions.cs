using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    internal static class BenchmarkInfoExtensions
    {
        public static IEnumerable<BenchmarkInfo> PreservePinned(this IEnumerable<BenchmarkInfo> benchmarkInfos, int maxRuns)
        {
            benchmarkInfos = benchmarkInfos.NullToEmpty();
            var survivors = PreservePinnedInner(benchmarkInfos, maxRuns).ToList();

            var victims = survivors.Count - maxRuns;
            if (victims > 0)
            {
                return CullPreserved(survivors, victims).Reverse();
            }
            return survivors;
            
        }

        public static IEnumerable<BenchmarkInfo> PinBest(this IEnumerable<BenchmarkInfo> values, IBenchmarkStatisticAccessorProvider statistics)
        {
            var benchmarkInfos = values.NullToEmpty();

            var stats = statistics.GetAccessorInfos()
                .Select(i => statistics.GetNullableAccessor(i.Name))
                .ToList();
            
            UnpinAll(benchmarkInfos);
            

            var resultGroups = benchmarkInfos.SelectMany(bi =>
                    bi.Runs.NullToEmpty().SelectMany(bri =>
                        bri.Results.NullToEmpty().Select(br => new {Info = bi, Run = br})))
                .GroupBy(a => a.Run.FullName);

            foreach (var grp in resultGroups)
            {
                var pairs = grp.ToList();

                foreach (var stat in stats)
                {
                    var scored = pairs
                        .Select(a => new {a.Info, a.Run, Score = stat(a.Run) })
                        .Where(a => a.Score.HasValue)
                        .OrderBy(a => a.Score.Value)
                        .ToList();
                    if (scored.Count > 0)
                    {
                        var bestScore = scored[0].Score;
                        var best = scored.TakeWhile(a => a.Score <= bestScore);
                        foreach (var x in best)
                        {
                            x.Info.Pinned = true;
                        }
                    }
                }
            }
            
            return benchmarkInfos;
        }

        
        public static BenchmarkInfo TrimRunsByFilter(this BenchmarkInfo benchmarkInfo, IList<string> filters)
        {
            benchmarkInfo.ArgNotNull(nameof(benchmarkInfo));

            if (filters.IsNullOrEmpty()) return benchmarkInfo;

            var runs = benchmarkInfo.Runs.NullToEmpty()
                .Select(r =>
                {
                    var results = r.Results.NullToEmpty()
                        .Where(br => IsIncluded(br, filters))
                        .ToList();

                    if (results.Count == 0)
                    {
                        return null;
                    }
                    r.Results = results;
                    return r;
                })
                .Where(bri => bri != null)
                .ToList();

            benchmarkInfo.Runs = runs;
            return benchmarkInfo;
        }

        private static void UnpinAll(IEnumerable<BenchmarkInfo> benchmarkInfos)
        {
            foreach (var benchmarkInfo in benchmarkInfos)
            {
                benchmarkInfo.Pinned = false;
            }
        }
        
        private static IEnumerable<BenchmarkInfo> PreservePinnedInner(this IEnumerable<BenchmarkInfo> benchmarkInfos, int maxRuns)
        {
            var yielded = 0;
            foreach (var info in benchmarkInfos)
            {
                if (info.Pinned)
                {
                    yield return info;
                    yielded++;
                }
                else if (yielded < maxRuns)
                {
                    yield return info;
                    yielded++;
                }
            }
        }

        private static IEnumerable<BenchmarkInfo> CullPreserved(this IList<BenchmarkInfo> benchmarkInfos, int victims)
        {
            for (var survivorsCount = benchmarkInfos.Count - 1; survivorsCount >= 0; survivorsCount--)
            {
                var survivor = benchmarkInfos[survivorsCount];
                if (!survivor.Pinned && victims > 0)
                {
                    victims--;
                }
                else
                {
                    yield return survivor;
                }
            }
        }

        internal static bool IsIncluded(this BenchmarkResult result, IList<string> filters) =>
            filters.Any(f => result.Namespace.IsMatch(f) ||
                             result.Type.IsMatch(f) ||
                             result.Method.IsMatch(f));
    }
}

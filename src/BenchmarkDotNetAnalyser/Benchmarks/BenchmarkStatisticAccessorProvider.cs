using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public class BenchmarkStatisticAccessorProvider : IBenchmarkStatisticAccessorProvider
    {
        private readonly Dictionary<string, Func<BenchmarkResult, decimal>> _accessors;
        private readonly string _defaultAccessorName = nameof(BenchmarkResult.Mean);

        public BenchmarkStatisticAccessorProvider()
        {
            _accessors =
                new Dictionary<string, Func<BenchmarkResult, decimal>>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {nameof(BenchmarkResult.Mean), br => br.Mean.GetValueOrDefault()},
                    {nameof(BenchmarkResult.Median), br => br.Median.GetValueOrDefault()},
                    {nameof(BenchmarkResult.Min), br => br.Min.GetValueOrDefault()},
                    {nameof(BenchmarkResult.Max), br => br.Max.GetValueOrDefault()},
                };
        }

        public Func<BenchmarkResult, decimal> GetAccessor(string statistic)
        {
            Func<BenchmarkResult, decimal> accessor = null;
            
            if (statistic != null)
            {
                _accessors.TryGetValue(statistic, out accessor);
            }

            if (accessor == null) accessor = _accessors[_defaultAccessorName];

            return accessor;
        }

        public IEnumerable<BenchmarkStatisticAccessorInfo> GetAccessorInfos() => 
            _accessors.OrderBy(x => x.Key)
                      .Select(x => new BenchmarkStatisticAccessorInfo(x.Key, _defaultAccessorName == x.Key));
    }
}

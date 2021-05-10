using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public class BenchmarkStatisticAccessorProvider : IBenchmarkStatisticAccessorProvider
    {
        private readonly Dictionary<string, Func<BenchmarkResult, decimal>> _accessors;
        private readonly string _defaultAccessorName = nameof(BenchmarkResult.MeanTime);

        public BenchmarkStatisticAccessorProvider()
        {
            _accessors =
                new Dictionary<string, Func<BenchmarkResult, decimal>>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {nameof(BenchmarkResult.MeanTime), br => br.MeanTime.GetValueOrDefault()},
                    {nameof(BenchmarkResult.MedianTime), br => br.MedianTime.GetValueOrDefault()},
                    {nameof(BenchmarkResult.MinTime), br => br.MinTime.GetValueOrDefault()},
                    {nameof(BenchmarkResult.MaxTime), br => br.MaxTime.GetValueOrDefault()},
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

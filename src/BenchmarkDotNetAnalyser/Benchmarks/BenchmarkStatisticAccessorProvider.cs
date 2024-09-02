using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public class BenchmarkStatisticAccessorProvider : IBenchmarkStatisticAccessorProvider
    {
        private readonly Dictionary<string, Func<BenchmarkResult, decimal?>> _accessors;
        private readonly string _defaultAccessorName = nameof(BenchmarkResult.MeanTime);
        
        public BenchmarkStatisticAccessorProvider()
        {
            _accessors =
                new Dictionary<string, Func<BenchmarkResult, decimal?>>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {nameof(BenchmarkResult.MeanTime), br => br.MeanTime},
                    {nameof(BenchmarkResult.MedianTime), br => br.MedianTime},
                    {nameof(BenchmarkResult.MinTime), br => br.MinTime},
                    {nameof(BenchmarkResult.MaxTime), br => br.MaxTime},
                    {nameof(BenchmarkResult.Q1Time), br => br.Q1Time},
                    {nameof(BenchmarkResult.Q3Time), br => br.Q3Time},
                    {nameof(BenchmarkResult.Gen0Collections), br => br.Gen0Collections},
                    {nameof(BenchmarkResult.Gen1Collections), br => br.Gen1Collections},
                    {nameof(BenchmarkResult.Gen2Collections), br => br.Gen2Collections},
                    {nameof(BenchmarkResult.TotalOps), br => br.TotalOps},
                    {nameof(BenchmarkResult.BytesAllocatedPerOp), br => br.BytesAllocatedPerOp},
                };
        }

        public Func<BenchmarkResult, decimal> GetAccessor(string statistic)
        {
            var result = GetNullableAccessor(statistic);

            return br => result(br).GetValueOrDefault();
        }

        public Func<BenchmarkResult, decimal?> GetNullableAccessor(string statistic)
        {
            Func<BenchmarkResult, decimal?> accessor = null;
            
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

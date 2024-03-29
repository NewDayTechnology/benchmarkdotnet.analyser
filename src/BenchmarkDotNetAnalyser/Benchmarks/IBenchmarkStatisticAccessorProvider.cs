﻿using System;
using System.Collections.Generic;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public interface IBenchmarkStatisticAccessorProvider
    {
        Func<BenchmarkResult, decimal> GetAccessor(string statistic);
        Func<BenchmarkResult, decimal?> GetNullableAccessor(string statistic);
        IEnumerable<BenchmarkStatisticAccessorInfo> GetAccessorInfos();
    }
}
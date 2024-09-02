using System;
using System.Collections.Generic;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public class BenchmarkRecordCell
    {
        public DateTimeOffset Creation { get; set; }

        public string BuildNumber { get; set; }
        
        public string BranchName { get; set; }
        public string CommitSha { get; set; }
        
        public string BuildUrl { get; set; }
        
        public IList<string> Tags { get; set; }
        
        public decimal? MeanTime { get; set; }
        
        public decimal? MinTime { get; set; }

        public decimal? Q1Time { get; set; }

        public decimal? MedianTime { get; set; }

        public decimal? Q3Time { get; set; }

        public decimal? MaxTime { get; set; }

        public decimal? Gen0Collections { get; set; }
        public decimal? Gen1Collections { get; set; }
        public decimal? Gen2Collections { get; set; }
        public decimal? BytesAllocatedPerOp { get; set; }
    }
}

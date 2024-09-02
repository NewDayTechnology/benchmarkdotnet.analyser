using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Reporting
{
    internal static class BenchmarkRecordExtensions
    {
        public static IEnumerable<BenchmarkCsvRow> ToCsvRows(this IEnumerable<BenchmarkRecord> records) =>
            records.ArgNotNull(nameof(records))
                .SelectMany(r => r.Cells.Select(c =>
                new BenchmarkCsvRow()
                {
                    FullName = r.FullName, 
                    Namespace = r.Namespace, 
                    Type = r.Type, 
                    Method = r.Method, 
                    Parameters = r.Parameters,
                    Creation = c.Creation.UtcDateTime, 
                    BuildNumber = c.BuildNumber, 
                    CommitSha = c.CommitSha,
                    BuildUrl = c.BuildUrl, 
                    BranchName = c.BranchName,
                    Tag1 = c.Tags.NullToEmpty().Take(1).FirstOrDefault(),
                    Tag2 = c.Tags.NullToEmpty().Skip(1).Take(1).FirstOrDefault(),
                    Tag3 = c.Tags.NullToEmpty().Skip(2).Take(1).FirstOrDefault(),
                    Tag4 = c.Tags.NullToEmpty().Skip(3).Take(1).FirstOrDefault(),
                    Tag5 = c.Tags.NullToEmpty().Skip(4).Take(1).FirstOrDefault(),
                    Tag6 = c.Tags.NullToEmpty().Skip(5).Take(1).FirstOrDefault(),
                    MinTime = c.MinTime, 
                    MaxTime = c.MaxTime, 
                    MeanTime = c.MeanTime, 
                    MedianTime = c.MedianTime, 
                    Q1Time = c.Q1Time, 
                    Q3Time = c.Q3Time,
                    Gen0Collections = c.Gen0Collections,
                    Gen1Collections = c.Gen1Collections,
                    Gen2Collections = c.Gen2Collections,
                    BytesAllocatedPerOp = c.BytesAllocatedPerOp
                }));
    }
}

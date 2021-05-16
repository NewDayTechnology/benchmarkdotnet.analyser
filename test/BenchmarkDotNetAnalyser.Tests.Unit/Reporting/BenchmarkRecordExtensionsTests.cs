using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Reporting;
using FsCheck;
using FsCheck.Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Reporting
{
    public class BenchmarkRecordExtensionsTests
    {
        [Property(Verbose = true)]
        public bool ToCsvRows_Mapped(PositiveInt recordCount, PositiveInt cellCount)
        {
            Func<int, int, decimal> f = (i, j) => (decimal) (i + j);

            var records = Enumerable.Range(1, recordCount.Get)
                .Select(i =>
                    new BenchmarkRecord()
                    {
                        FullName = $"fullName{i}",
                        Cells = Enumerable.Range(1, cellCount.Get)
                            .Select(j => new BenchmarkRecordCell()
                            {
                                BranchName = $"branch{i}",
                                MeanTime = f(i, j),
                                MinTime = f(i, j),
                                MaxTime = f(i, j),
                                MedianTime = f(i, j),
                                Q1Time = f(i, j),
                                Q3Time = f(i, j),
                            }).ToList(),
                    }
                );

            var rows = records.ToCsvRows().ToList();
            var expected = Enumerable.Range(1, recordCount.Get)
                .SelectMany(i => Enumerable.Range(1, cellCount.Get)
                    .Select(j => f(i,j)));
            var actual = rows.Select(r => r.MeanTime.Value);

            return expected.SequenceEqual(actual);
        }

        [Property(Verbose = true)]
        public bool ToCsvRows_TagsMapped(PositiveInt tagCount)
        {
            var tags = Enumerable.Range(1, tagCount.Get)
                .Select(i => Guid.NewGuid().ToString())
                .ToList();
            var rec = new BenchmarkRecord()
            {
                FullName = "fullName",
                Cells = new[]
                {
                    new BenchmarkRecordCell()
                    {
                        Tags = tags,
                    }
                },
            };

            var row = rec.Singleton().ToCsvRows().Single();

            var expected = tags.Take(6).ToList();
            var padding = 6 - tagCount.Get;
            if (padding > 0)
            {
                expected.AddRange(Enumerable.Range(1, padding).Select(_ => (string)null));
            }
            var actual = new[] {row.Tag1, row.Tag2, row.Tag3, row.Tag4, row.Tag5, row.Tag6};

            return actual.SequenceEqual(expected);
        }
    }
}

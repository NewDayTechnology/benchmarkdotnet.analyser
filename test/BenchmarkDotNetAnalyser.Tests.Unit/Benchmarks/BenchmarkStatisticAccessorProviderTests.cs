using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Benchmarks
{
    public class BenchmarkStatisticAccessorProviderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData(" ")]
        public void GetAccessor_NullName_DefaultReturned(string name)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetAccessor(name);

            a.Should().NotBeNull();
        }

        [Theory]
        [InlineData(nameof(BenchmarkResult.MaxTime), true)]
        [InlineData(nameof(BenchmarkResult.MaxTime), false)]
        [InlineData(nameof(BenchmarkResult.MinTime), true)]
        [InlineData(nameof(BenchmarkResult.MinTime), false)]
        [InlineData(nameof(BenchmarkResult.MedianTime), true)]
        [InlineData(nameof(BenchmarkResult.MedianTime), false)]
        [InlineData(nameof(BenchmarkResult.MeanTime), true)]
        [InlineData(nameof(BenchmarkResult.MeanTime), false)]
        [InlineData(nameof(BenchmarkResult.Gen0Collections), true)]
        [InlineData(nameof(BenchmarkResult.Gen0Collections), false)]
        [InlineData(nameof(BenchmarkResult.BytesAllocatedPerOp), true)]
        [InlineData(nameof(BenchmarkResult.BytesAllocatedPerOp), false)]
        public void GetAccessor_AccessorFound(string name, bool upper)
        {
            name = upper ? name.ToUpper() : name;
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetAccessor(name);

            a.Should().NotBeNull();
        }

        [Property(Verbose = true)]
        public bool GetAccessor_NonEmptyStringReturnsDefault(NonEmptyString name)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetAccessor(name.Get);

            return a != null;
        }

        [Theory]
        [InlineData(nameof(BenchmarkResult.MinTime), 1)]
        [InlineData(nameof(BenchmarkResult.MaxTime), 2)]
        [InlineData(nameof(BenchmarkResult.MeanTime), 3)]
        [InlineData(nameof(BenchmarkResult.MedianTime), 4)]
        [InlineData("", 3)]
        public void GetAccessor_AccessorFound_AccessorEvaluates(string name, decimal expected)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetAccessor(name);

            var br = new BenchmarkResult()
            {
                MinTime = 1,
                MaxTime = 2,
                MeanTime = 3,
                MedianTime = 4
            };

            var result = a(br);

            result.Should().Be(expected);
        }

        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData(" ")]
        public void GetNullableAccessor_NullName_DefaultReturned(string name)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetNullableAccessor(name);

            a.Should().NotBeNull();
        }

        [Theory]
        [InlineData(nameof(BenchmarkResult.MaxTime), true)]
        [InlineData(nameof(BenchmarkResult.MaxTime), false)]
        [InlineData(nameof(BenchmarkResult.MinTime), true)]
        [InlineData(nameof(BenchmarkResult.MinTime), false)]
        [InlineData(nameof(BenchmarkResult.MedianTime), true)]
        [InlineData(nameof(BenchmarkResult.MedianTime), false)]
        [InlineData(nameof(BenchmarkResult.MeanTime), true)]
        [InlineData(nameof(BenchmarkResult.MeanTime), false)]
        [InlineData(nameof(BenchmarkResult.Gen0Collections), true)]
        [InlineData(nameof(BenchmarkResult.Gen0Collections), false)]
        [InlineData(nameof(BenchmarkResult.BytesAllocatedPerOp), true)]
        [InlineData(nameof(BenchmarkResult.BytesAllocatedPerOp), false)]
        public void GetNullableAccessor_AccessorFound(string name, bool upper)
        {
            name = upper ? name.ToUpper() : name;
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetNullableAccessor(name);

            a.Should().NotBeNull();
        }

        [Property(Verbose = true)]
        public bool GetNullableAccessor_NonEmptyStringReturnsDefault(NonEmptyString name)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetNullableAccessor(name.Get);

            return a != null;
        }

        [Theory]
        [InlineData(nameof(BenchmarkResult.MinTime), 1)]
        [InlineData(nameof(BenchmarkResult.MaxTime), 2)]
        [InlineData(nameof(BenchmarkResult.MeanTime), 3)]
        [InlineData(nameof(BenchmarkResult.MedianTime), 4)]
        [InlineData(nameof(BenchmarkResult.Gen0Collections), 5)]
        [InlineData(nameof(BenchmarkResult.Gen1Collections), 6)]
        [InlineData(nameof(BenchmarkResult.Gen2Collections), 7)]
        [InlineData(nameof(BenchmarkResult.TotalOps), 8)]
        [InlineData(nameof(BenchmarkResult.BytesAllocatedPerOp), 9)]
        [InlineData("", 3)]
        public void GetNullableAccessor_AccessorFound_AccessorEvaluates(string name, decimal expected)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetNullableAccessor(name);

            var br = new BenchmarkResult()
            {
                MinTime = expected,
                MaxTime = expected,
                MeanTime = expected,
                MedianTime = expected,
                Gen0Collections = expected,
                Gen1Collections = expected,
                Gen2Collections = expected,
                TotalOps = expected,
                BytesAllocatedPerOp = expected,
            };

            var result = a(br);

            result.Should().Be((decimal)expected);
        }

        [Theory]
        [InlineData(nameof(BenchmarkResult.MinTime))]
        [InlineData(nameof(BenchmarkResult.MaxTime))]
        [InlineData(nameof(BenchmarkResult.MeanTime))]
        [InlineData(nameof(BenchmarkResult.MedianTime))]
        [InlineData(nameof(BenchmarkResult.Gen0Collections))]
        [InlineData(nameof(BenchmarkResult.Gen1Collections))]
        [InlineData(nameof(BenchmarkResult.Gen2Collections))]
        [InlineData(nameof(BenchmarkResult.TotalOps))]
        [InlineData(nameof(BenchmarkResult.BytesAllocatedPerOp))]
        [InlineData("")]
        public void GetNullableAccessor_AccessorFound_AccessorEvaluates_2(string name)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetNullableAccessor(name);

            var br = new BenchmarkResult()
            {
                MinTime = default,
                MaxTime = default,
                MeanTime = default,
                MedianTime = default,
                Gen0Collections = default,
                Gen1Collections = default,
                Gen2Collections = default,
                TotalOps = default,
                BytesAllocatedPerOp = default,
            };

            var result = a(br);

            result.Should().BeNull();
        }

        [Fact]
        public void GetAccessorInfos_UniquesReturned()
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var result = p.GetAccessorInfos().ToList();

            result.Count.Should().BeGreaterThan(0);
            result.Distinct().Should().BeEquivalentTo(result);
            result.All(s => !string.IsNullOrWhiteSpace(s.Name)).Should().BeTrue();
        }

        [Fact]
        public void GetAccessorInfos_OrderedReturn()
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var result = p.GetAccessorInfos().ToList();
            
            result.Should().BeInAscendingOrder(x => x.Name);
        }

        [Fact]
        public void GetAccessorInfos_SingleIsDefault()
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var result = p.GetAccessorInfos().ToList();

            result.SingleOrDefault(x => x.IsDefault).Should().NotBeNull();
        }
    }
}

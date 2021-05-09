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
        [InlineData(nameof(BenchmarkResult.Max), true)]
        [InlineData(nameof(BenchmarkResult.Max), false)]
        [InlineData(nameof(BenchmarkResult.Min), true)]
        [InlineData(nameof(BenchmarkResult.Min), false)]
        [InlineData(nameof(BenchmarkResult.Median), true)]
        [InlineData(nameof(BenchmarkResult.Median), false)]
        [InlineData(nameof(BenchmarkResult.Mean), true)]
        [InlineData(nameof(BenchmarkResult.Mean), false)]
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
        [InlineData(nameof(BenchmarkResult.Min), 1)]
        [InlineData(nameof(BenchmarkResult.Max), 2)]
        [InlineData(nameof(BenchmarkResult.Mean), 3)]
        [InlineData(nameof(BenchmarkResult.Median), 4)]
        [InlineData("", 3)]
        public void GetAccessor_AccessorFound_AccessorEvaluates(string name, decimal expected)
        {
            var p = new BenchmarkStatisticAccessorProvider();
            var a = p.GetAccessor(name);

            var br = new BenchmarkResult()
            {
                Min = 1,
                Max = 2,
                Mean = 3,
                Median = 4
            };

            var result = a(br);

            result.Should().Be(expected);
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

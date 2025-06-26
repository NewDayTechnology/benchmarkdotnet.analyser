using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.IO;
using Shouldly;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.Classes.Benchmarks
{
    public class BenchmarkParserTests
    {
        private readonly string[] _benchmarkTypeNamespaces;
        private readonly string[] _benchmarkTypeNames;
        private readonly string[] _benchmarkMethods;
        private readonly string[] _benchmarkTypeFullNames;

        public BenchmarkParserTests()
        {
            var benchmarkMembers = typeof(BenchmarkDotNetAnalyser.SampleBenchmarks.Program)
                .Assembly.GetTypes()
                .SelectMany(t => t.GetMemberAttributePairs<BenchmarkAttribute>())
                .ToArray();

            _benchmarkTypeNamespaces = benchmarkMembers.Select(t => t.Item1.DeclaringType.Namespace)
                                                .Distinct()
                                                .ToArray();

            _benchmarkTypeNames = benchmarkMembers.Select(t => t.Item1.DeclaringType.Name)
                                                .Distinct()
                                                .ToArray();

            _benchmarkTypeFullNames = benchmarkMembers.Select(t => t.Item1.DeclaringType.FullName)
                .Distinct()
                .ToArray();

            _benchmarkMethods = benchmarkMembers.Select(t => t.Item1.Name).Distinct().ToArray();
        }

        public static IEnumerable<object[]> GetFilePaths() => IOHelper.GetSourceResultFilePaths().Select(x => new[] {x});

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetCreation_ValueParsed(string filePath)
        {
            var json = await FileReader.ReadAsync(filePath);

            var result = new BenchmarkParser(json).GetCreation();

            result.ShouldBeGreaterThan(default);
            result.Year.ShouldBeGreaterThan(2020);
            result.Year.ShouldBeLessThan(2100);
        }

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetBenchmarkEnvironment_ValueParsed(string filePath)
        {
            var json = await FileReader.ReadAsync(filePath);

            var jo = Newtonsoft.Json.Linq.JObject.Parse(json)["HostEnvironmentInfo"];
            var result = new BenchmarkParser(json).GetBenchmarkEnvironment();

            result.ShouldNotBeNull();
            result.BenchmarkDotNetVersion.ShouldBe(jo.GetStringValue("BenchmarkDotNetVersion"));
            result.BenchmarkDotNetVersion.ShouldNotBeNullOrWhiteSpace();
            result.DotNetCliVersion.ShouldBe(jo.Value<string>("DotNetCliVersion"));
            result.DotNetCliVersion.ShouldNotBeNullOrWhiteSpace();
            result.DotNetRuntimeVersion.ShouldBe(jo.Value<string>("RuntimeVersion"));
            result.DotNetRuntimeVersion.ShouldNotBeNullOrWhiteSpace();
            result.OsVersion.ShouldBe(jo.Value<string>("OsVersion"));
            result.OsVersion.ShouldNotBeNullOrWhiteSpace();
            result.ProcessorName.ShouldBe(jo.Value<string>("ProcessorName"));
            result.ProcessorName.ShouldNotBeNullOrWhiteSpace();

            result.PhysicalProcessorCount.ShouldBe(jo.Value<int>("PhysicalProcessorCount"));
            result.PhysicalProcessorCount.ShouldBeGreaterThan(0);
            result.LogicalCoreCount.ShouldBe(jo.Value<int>("LogicalCoreCount"));
            result.LogicalCoreCount.ShouldBeGreaterThan(0);
            result.MachineArchitecture.ShouldBe(jo.Value<string>("Architecture"));
            result.MachineArchitecture.ShouldNotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetBenchmarkResults_ValuesParsed(string filePath)
        {
            var json = await FileReader.ReadAsync(filePath);
            var results = new BenchmarkParser(json).GetBenchmarkResults().ToList();

            results.Count.ShouldBeGreaterThan(0);
            
            foreach (var result in results)
            {
                result.FullName.ShouldNotBeNullOrWhiteSpace();
                _benchmarkTypeFullNames.Any(x => result.FullName.Contains(x)).ShouldBeTrue();
                result.Namespace.ShouldNotBeNullOrWhiteSpace();
                _benchmarkTypeNamespaces.Any(x => result.Namespace.Contains(x)).ShouldBeTrue();
                result.Type.ShouldNotBeNullOrWhiteSpace();
                _benchmarkTypeNames.Any(x => result.Type.Contains(x)).ShouldBeTrue();
                result.Method.ShouldNotBeNullOrWhiteSpace();
                _benchmarkMethods.Any(x => result.Method.Contains(x)).ShouldBeTrue();
                result.Parameters.ShouldNotBeNull();

                result.MaxTime.ShouldNotBeNull();
                result.MaxTime.Value.ShouldBeGreaterThanOrEqualTo(0M);
                result.MinTime.ShouldNotBeNull();
                result.MinTime.Value.ShouldBeGreaterThanOrEqualTo(0M);
                result.MeanTime.ShouldNotBeNull();
                result.MeanTime.Value.ShouldBeGreaterThanOrEqualTo(0M);
                result.MedianTime.ShouldNotBeNull();
                result.MedianTime.Value.ShouldBeGreaterThanOrEqualTo(0M);
                result.Q1Time.ShouldNotBeNull();
                result.Q1Time.Value.ShouldBeGreaterThanOrEqualTo(0M);
                result.Q3Time.ShouldNotBeNull();
                result.Q3Time.Value.ShouldBeGreaterThanOrEqualTo(0M);
            }
        }
    }
}

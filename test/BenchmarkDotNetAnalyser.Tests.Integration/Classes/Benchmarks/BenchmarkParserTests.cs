using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.IO;
using FluentAssertions;
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

            result.Should().BeAfter(default);
            result.Year.Should().BeGreaterThan(2020);
            result.Year.Should().BeLessThan(2100);
        }

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetBenchmarkEnvironment_ValueParsed(string filePath)
        {
            var json = await FileReader.ReadAsync(filePath);

            var jo = Newtonsoft.Json.Linq.JObject.Parse(json)["HostEnvironmentInfo"];
            var result = new BenchmarkParser(json).GetBenchmarkEnvironment();

            result.Should().NotBeNull();
            result.BenchmarkDotNetVersion.Should().Be(jo.GetStringValue("BenchmarkDotNetVersion")).And.NotBeNullOrWhiteSpace();
            result.DotNetCliVersion.Should().Be(jo.Value<string>("DotNetCliVersion")).And.NotBeNullOrWhiteSpace();
            result.DotNetRuntimeVersion.Should().Be(jo.Value<string>("RuntimeVersion")).And.NotBeNullOrWhiteSpace();
            result.OsVersion.Should().Be(jo.Value<string>("OsVersion")).And.NotBeNullOrWhiteSpace();
            result.ProcessorName.Should().Be(jo.Value<string>("ProcessorName")).And.NotBeNullOrWhiteSpace();

            result.PhysicalProcessorCount.Should().Be(jo.Value<int>("PhysicalProcessorCount")).And.BeGreaterThan(0);
            result.LogicalCoreCount.Should().Be(jo.Value<int>("LogicalCoreCount")).And.BeGreaterThan(0);
            result.MachineArchitecture.Should().Be(jo.Value<string>("Architecture")).And.NotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(GetFilePaths))]
        public async Task GetBenchmarkResults_ValuesParsed(string filePath)
        {
            var json = await FileReader.ReadAsync(filePath);
            var results = new BenchmarkParser(json).GetBenchmarkResults().ToList();

            results.Count.Should().BeGreaterThan(0);
            
            foreach (var result in results)
            {
                result.FullName.Should().NotBeNullOrWhiteSpace().And.ContainAny(_benchmarkTypeFullNames);
                result.Namespace.Should().NotBeNullOrWhiteSpace().And.ContainAny(_benchmarkTypeNamespaces);
                result.Type.Should().NotBeNullOrWhiteSpace().And.ContainAny(_benchmarkTypeNames);
                result.Method.Should().NotBeNullOrWhiteSpace().And.ContainAny(_benchmarkMethods);
                result.Parameters.Should().NotBeNull();

                result.MaxTime.Should().HaveValue().And.BeGreaterOrEqualTo(0M);
                result.MinTime.Should().HaveValue().And.BeGreaterOrEqualTo(0M);
                result.MeanTime.Should().HaveValue().And.BeGreaterOrEqualTo(0M);
                result.MedianTime.Should().HaveValue().And.BeGreaterOrEqualTo(0M);
                result.Q1Time.Should().HaveValue().And.BeGreaterOrEqualTo(0M);
                result.Q3Time.Should().HaveValue().And.BeGreaterOrEqualTo(0M);
            }
        }
    }
}

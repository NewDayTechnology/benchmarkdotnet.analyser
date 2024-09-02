using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Benchmarks
{
    public class BenchmarkParserTests
    {
        
        [Fact]
        public void GetBenchmarkEnvironment_GetCreation()
        {
            var json = @"{ ""Title"":""benchmarkdotnetdemo.FibonacciBenchmark-20210316-173743"" }";

            var p = new BenchmarkParser(json);

            var ct = p.GetCreation();

            var expected = new DateTime(2021, 3, 16, 17, 37, 43, DateTimeKind.Utc);

            ct.Should().Be(expected);
        }

        [Theory]
        [InlineData("bdn1", "0.1.2", "win10-x64", "xeon E5-2673", "X64", 1, 2, 4, ".NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614)", "5.0.201")]
        [InlineData("bdn2", "1.2.3", "ubuntu-x64", "E5-2673", "x64", 2, 4, 8, ".NET Core 5.0.4", "5.0.201")]
        public void GetBenchmarkEnvironment_ValuesParsed(string caption, string bdnVersion, string os, string proc, 
                                                         string cpuArchitecture, int physicalProcs, int physicalCores, int logicalCores,
                                                         string runtimeVersion, string cliVersion)
        {
            var hostInfo = new
            {
                HostEnvironmentInfo = new 
                {
                    BenchmarkDotNetCaption = caption,
                    BenchmarkDotNetVersion = bdnVersion,
                    OsVersion = os,
                    ProcessorName = proc,
                    PhysicalProcessorCount = physicalProcs,
                    PhysicalCoreCount = physicalCores,
                    LogicalCoreCount = logicalCores,
                    RuntimeVersion = runtimeVersion,
                    Architecture = cpuArchitecture,
                    DotNetCliVersion = cliVersion,
                }
            };

            var json = JsonConvert.SerializeObject(hostInfo);
            var env = new BenchmarkParser(json).GetBenchmarkEnvironment();

            env.Should().NotBeNull();
            env.BenchmarkDotNetVersion.Should().Be(hostInfo.HostEnvironmentInfo.BenchmarkDotNetVersion);
            env.OsVersion.Should().Be(hostInfo.HostEnvironmentInfo.OsVersion);
            env.ProcessorName.Should().Be(hostInfo.HostEnvironmentInfo.ProcessorName);
            env.LogicalCoreCount.Should().Be(hostInfo.HostEnvironmentInfo.LogicalCoreCount);
            env.PhysicalProcessorCount.Should().Be(hostInfo.HostEnvironmentInfo.PhysicalProcessorCount);
            env.MachineArchitecture.Should().Be(hostInfo.HostEnvironmentInfo.Architecture);
            env.DotNetCliVersion.Should().Be(hostInfo.HostEnvironmentInfo.DotNetCliVersion);
            env.DotNetRuntimeVersion.Should().Be(hostInfo.HostEnvironmentInfo.RuntimeVersion);
        }

        [Theory]
        [InlineData("fullName1", "benchmarknamespace1", "benchmarktype1", "benchmarkMethod1", "benchmarkparams1", 0.1)]
        [InlineData("fullName2", "benchmarknamespace2", "benchmarktype2", "benchmarkMethod2", "benchmarkparams2", 0.001)]
        public void GetBenchmarkResults_ValuesParsed(string fullName, string benchmarkNamespace, string benchmarkType,
                                                     string benchmarkMethod, string benchmarkParameters, decimal seedStats)
        {
            var benchmark = new
            {
                FullName = fullName,
                Namespace = benchmarkNamespace,
                Type = benchmarkType,
                Method = benchmarkMethod,
                Parameters = benchmarkParameters,
                Statistics = new
                {
                    Min = seedStats,
                    Max = seedStats * 1000,
                    Mean = seedStats * 100,
                    Median = seedStats * 10,
                    Q1 = seedStats * 5,
                    Q3 = seedStats * 500,
                },
                Memory = new
                {
                    Gen0Collections = seedStats * 1111,
                    Gen1Collections = seedStats * 111,
                    Gen2Collections = seedStats * 11,
                    TotalOperations = seedStats * 111000000,
                    BytesAllocatedPerOperation = seedStats * 42,
                }
            };

            var benchmarks = new
            {
                Benchmarks = new[] {benchmark},
            };

            var json = JsonConvert.SerializeObject(benchmarks);

            var results = new BenchmarkParser(json).GetBenchmarkResults().ToList();

            results[0].FullName.Should().Be(benchmark.FullName);
            results[0].Namespace.Should().Be(benchmark.Namespace);
            results[0].Type.Should().Be(benchmark.Type);
            results[0].Method.Should().Be(benchmark.Method);
            results[0].Parameters.Should().Be(benchmark.Parameters);
            results[0].MinTime.Should().Be(benchmark.Statistics.Min);
            results[0].MaxTime.Should().Be(benchmark.Statistics.Max);
            results[0].MeanTime.Should().Be(benchmark.Statistics.Mean);
            results[0].MedianTime.Should().Be(benchmark.Statistics.Median);
            results[0].Q1Time.Should().Be(benchmark.Statistics.Q1);
            results[0].Q3Time.Should().Be(benchmark.Statistics.Q3);

            results[0].Gen0Collections.Should().Be(benchmark.Memory.Gen0Collections);
            results[0].Gen1Collections.Should().Be(benchmark.Memory.Gen1Collections);
            results[0].Gen2Collections.Should().Be(benchmark.Memory.Gen2Collections);
            results[0].TotalOps.Should().Be(benchmark.Memory.TotalOperations);
            results[0].BytesAllocatedPerOp.Should().Be(benchmark.Memory.BytesAllocatedPerOperation);
        }

        [Theory]
        [InlineData("fullName1", "benchmarknamespace1", "benchmarktype1", "benchmarkMethod1", "benchmarkparams1", 0.1)]
        [InlineData("fullName2", "benchmarknamespace2", "benchmarktype2", "benchmarkMethod2", "benchmarkparams2", 0.001)]
        public void GetBenchmarkResults_WithoutMemory_ValuesParsed(string fullName, string benchmarkNamespace, string benchmarkType,
                                                     string benchmarkMethod, string benchmarkParameters, decimal seedStats)
        {
            var benchmark = new
            {
                FullName = fullName,
                Namespace = benchmarkNamespace,
                Type = benchmarkType,
                Method = benchmarkMethod,
                Parameters = benchmarkParameters,
                Statistics = new
                {
                    Min = seedStats,
                    Max = seedStats * 1000,
                    Mean = seedStats * 100,
                    Median = seedStats * 10,
                    Q1 = seedStats * 5,
                    Q3 = seedStats * 500,
                }
            };

            var benchmarks = new
            {
                Benchmarks = new[] { benchmark },
            };

            var json = JsonConvert.SerializeObject(benchmarks);

            var results = new BenchmarkParser(json).GetBenchmarkResults().ToList();

            results[0].FullName.Should().Be(benchmark.FullName);
            results[0].Namespace.Should().Be(benchmark.Namespace);
            results[0].Type.Should().Be(benchmark.Type);
            results[0].Method.Should().Be(benchmark.Method);
            results[0].Parameters.Should().Be(benchmark.Parameters);
            results[0].MinTime.Should().Be(benchmark.Statistics.Min);
            results[0].MaxTime.Should().Be(benchmark.Statistics.Max);
            results[0].MeanTime.Should().Be(benchmark.Statistics.Mean);
            results[0].MedianTime.Should().Be(benchmark.Statistics.Median);
            results[0].Q1Time.Should().Be(benchmark.Statistics.Q1);
            results[0].Q3Time.Should().Be(benchmark.Statistics.Q3);

            results[0].Gen0Collections.Should().BeNull();
            results[0].Gen1Collections.Should().BeNull(); ;
            results[0].Gen2Collections.Should().BeNull(); ;
            results[0].TotalOps.Should().BeNull(); ;
            results[0].BytesAllocatedPerOp.Should().BeNull();
        }


        [Theory]
        [InlineData("fullName1", "benchmarknamespace1", "benchmarktype1", "benchmarkMethod1", "benchmarkparams1")]
        [InlineData("fullName2", "benchmarknamespace2", "benchmarktype2", "benchmarkMethod2", "benchmarkparams2")]
        public void GetBenchmarkResults_NoStats_ValuesParsed(string fullName, string benchmarkNamespace, string benchmarkType,
                                                     string benchmarkMethod, string benchmarkParameters)
        {
            var benchmark = new
            {
                FullName = fullName,
                Namespace = benchmarkNamespace,
                Type = benchmarkType,
                Method = benchmarkMethod,
                Parameters = benchmarkParameters,
            };

            var benchmarks = new
            {
                Benchmarks = new[] {benchmark},
            };

            var json = JsonConvert.SerializeObject(benchmarks);

            var results = new BenchmarkParser(json).GetBenchmarkResults().ToList();

            results[0].FullName.Should().Be(benchmark.FullName);
            results[0].Namespace.Should().Be(benchmark.Namespace);
            results[0].Type.Should().Be(benchmark.Type);
            results[0].Method.Should().Be(benchmark.Method);
            results[0].Parameters.Should().Be(benchmark.Parameters);
            results[0].MinTime.Should().BeNull();
            results[0].MaxTime.Should().BeNull();
            results[0].MeanTime.Should().BeNull();
            results[0].MedianTime.Should().BeNull();
            results[0].Q1Time.Should().BeNull();
            results[0].Q3Time.Should().BeNull();
        }

    }
}

using System;
using BenchmarkDotNetAnalyser.Aggregation;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using BenchmarkDotNetAnalyser.IO;
using Shouldly;
using McMaster.Extensions.CommandLineUtils;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public class ProgramBootstrapTests
    {
        [Theory]
        [InlineData(typeof(IFileFinder))]
        [InlineData(typeof(IConsole))]
        [InlineData(typeof(ITelemetry))]
        [InlineData(typeof(IBenchmarkRunInfoProvider))]
        [InlineData(typeof(IBenchmarkInfoProvider))]
        [InlineData(typeof(IBenchmarkAggregator))]
        [InlineData(typeof(IAggregateBenchmarksExecutor))]
        [InlineData(typeof(IAnalyseBenchmarksExecutor))]
        public void CreateServiceCollection_ServiceInjected(Type serviceType)
        {
            var result = ProgramBootstrap.CreateServiceCollection();
            
            var service = result.GetService(serviceType);

            service.ShouldNotBeNull();
        }

        [Fact]
        public void GetDescription_SolidTextReturned()
        {
            var result = ProgramBootstrap.GetDescription();

            result.ShouldNotBeNullOrWhiteSpace();
        }
    }
}

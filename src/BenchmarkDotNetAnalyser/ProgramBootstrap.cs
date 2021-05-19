using System;
using System.Linq;
using System.Reflection;
using BenchmarkDotNetAnalyser.Aggregation;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using BenchmarkDotNetAnalyser.IO;
using BenchmarkDotNetAnalyser.Reporting;
using Crayon;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace BenchmarkDotNetAnalyser
{
    internal static class ProgramBootstrap
    {
        public static IServiceProvider CreateServiceCollection()
        {
            return new ServiceCollection()
                .AddSingleton<IFileFinder, FileFinder>()
                .AddSingleton<IConsole>(sp => PhysicalConsole.Singleton)
                .AddSingleton<ITelemetry, ConsoleTelemetry>()
                .AddSingleton<IBenchmarkRunInfoProvider, BenchmarkRunInfoJsonFileProvider>()
                .AddSingleton<IBenchmarkInfoProvider, BenchmarkInfoJsonFileProvider>()
                .AddSingleton<IBenchmarkAggregator, BenchmarkAggregator>()
                .AddSingleton<IAggregateBenchmarksCommandValidator, AggregateBenchmarksCommandValidator>()
                .AddSingleton<IAggregateBenchmarksExecutor, AggregateBenchmarksExecutor>()
                .AddSingleton<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>, AnalyseBenchmarksCommandValidator>()
                .AddSingleton<IAnalyseBenchmarksExecutor, AnalyseBenchmarksExecutor>()
                .AddSingleton<IReportBenchmarksCommandValidator, ReportBenchmarksCommandValidator>()
                .AddSingleton<IBenchmarkStatisticAccessorProvider, BenchmarkStatisticAccessorProvider>()
                .AddSingleton<IBenchmarkResultAnalysisReporter, TelemetryBenchmarkResultAnalysisReporter>()
                .AddSingleton<IReporterProvider, ReporterProvider>()
                .AddSingleton<ICsvFileWriter, CsvFileWriter>()
                .AddSingleton<IBenchmarkReader, BenchmarkReader>()
                .AddSingleton<IJsonFileWriter, JsonFileWriter>()
                .BuildServiceProvider();
        }

        public static string GetDescription()
        {
            var attrs = typeof(ProgramBootstrap).Assembly.GetCustomAttributes();

            return new[]
                {
                    Output.Bright.Magenta(Resources.ProgramTitle),
                    attrs.GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description),
                    "",
                    $"{Output.Bright.Yellow(attrs.GetAttributeValue<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion).Format("Version {0}"))}{Output.Bright.Green(" beta ")}",
                    Output.Bright.Yellow(attrs.GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright)),
                    Output.Bright.Yellow("You can find the repository at https://github.com/NewDayTechnology/benchmarkdotnet.analyser"),
                }.Where(x => x != null)
                .Join(Environment.NewLine);
        }
    }
}

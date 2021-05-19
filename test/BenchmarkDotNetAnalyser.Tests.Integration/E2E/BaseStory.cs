using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Aggregation;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using BenchmarkDotNetAnalyser.IO;
using BenchmarkDotNetAnalyser.Reporting;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NSubstitute;

namespace BenchmarkDotNetAnalyser.Tests.Integration.E2E
{
    public class BaseStory
    {
        private readonly string _workingPath;
        private string _newRunPath;
        private string _newAggPath;
        private string _outputAggPath;
        private AggregateBenchmarksExecutorArgs _aggregateArgs;
        private bool _aggregateResult;
        private IList<BenchmarkInfo> _newBenchmarkInfos;
        private IList<BenchmarkRunResults> _benchmarkRunResults;
        private int _newRunFiles;
        private AnalyseBenchmarksExecutorArgs _analysisArgs;
        private readonly ITelemetry _telemetry;
        private BenchmarkResultAnalysis _analysisResult;
        private IList<string> _reportResult;
        private string _reportsOutputPath;

        public BaseStory()
        {
            _workingPath = IOHelper.CreateTempFolder();
            _telemetry = Substitute.For<ITelemetry>();
        }
        
        public void RunsDirectoryCreated()
        {
            _newRunPath = IOHelper.CreateTempFolder(_workingPath, "NewRun");
        }

        public void RunDataFilesAreCopiedToRunsDirectory(IList<string> filePaths)
        {
            IOHelper.CopyFiles(_newRunPath, filePaths);
            _newRunFiles = filePaths.Count;
        }

        public void AggregregatesDirectoryCreated()
        {
            _newAggPath = IOHelper.CreateTempFolder(_workingPath, "Aggregates");
            _outputAggPath = _newAggPath;
        }

        public void AnAggregateOutputDirectoryCreated()
        {
            _outputAggPath = IOHelper.CreateTempFolder(_workingPath, "Output");
        }
        
        public void AggregateArgumentsAreCreatedWithRuns(int runs)
        {
            _aggregateArgs = new AggregateBenchmarksExecutorArgs()
            {
                BenchmarkRuns = runs,
                AggregatedBenchmarksPath = _newAggPath,
                BuildNumber = "0.1.2",
                BranchName = "test",
                BuildUri = "http://localhost",
                NewBenchmarksPath = _newRunPath,
                OutputAggregatesPath = _outputAggPath,
                Tags = Enumerable.Range(1, 3).Select(i => $"Tag_{i}").ToList(),
            };
        }

        public async Task AggregationExecutorExecuted(bool expectedResult = true)
        {
            var aggregator = new AggregateBenchmarksExecutor(_telemetry,
                new FileFinder(),
                new BenchmarkRunInfoJsonFileProvider(),
                new BenchmarkInfoJsonFileProvider(),
                new BenchmarkAggregator(new BenchmarkStatisticAccessorProvider()));

            _aggregateResult = await aggregator.ExecuteAsync(_aggregateArgs);

            _aggregateResult.Should().Be(expectedResult);
        }

        public async Task AggregatedBenchmarkInfosFetched()
        {
            var infoReader = new BenchmarkInfoJsonFileProvider();

            _newBenchmarkInfos = await infoReader.GetBenchmarkInfosAsync(_outputAggPath);
        }

        public void SingletonBenchmarkInfoIsChecked()
        {
            _newBenchmarkInfos.Count.Should().Be(1);

            _newBenchmarkInfos[0].BranchName.Should().Be(_aggregateArgs.BranchName);
            _newBenchmarkInfos[0].BuildUri.Should().Be(_aggregateArgs.BuildUri);
            _newBenchmarkInfos[0].Tags.Should().BeEquivalentTo(_aggregateArgs.Tags);

            _newBenchmarkInfos[0].Runs.Should().HaveCount(_newRunFiles);
            
            _newBenchmarkInfos[0].Runs.All(p => !string.IsNullOrWhiteSpace(p.BenchmarkDotNetVersion)).Should().BeTrue();
            _newBenchmarkInfos[0].Runs.All(p => p.Creation > DateTimeOffset.MinValue).Should().BeTrue();
        }

        public void MultipleBenchmarkInfosAreChecked()
        {
            _newBenchmarkInfos.Count.Should().Be(_aggregateArgs.BenchmarkRuns);

            foreach (var bi in _newBenchmarkInfos)
            {
                bi.BranchName.Should().Be(_aggregateArgs.BranchName);
                bi.BuildUri.Should().Be(_aggregateArgs.BuildUri);
                bi.Tags.Should().BeEquivalentTo(_aggregateArgs.Tags);

                bi.Runs.Should().HaveCount(_newRunFiles);

                bi.Runs.All(p => !string.IsNullOrWhiteSpace(p.BenchmarkDotNetVersion)).Should().BeTrue();
                bi.Runs.All(p => p.Creation > DateTimeOffset.MinValue).Should().BeTrue();
            }
        }
        
        public void BenchmarkRunResultsRetrieved()
        {
            var rdr = new BenchmarkRunResultsReader();

            _benchmarkRunResults = rdr.GetBenchmarkResults(_newBenchmarkInfos);
        }

        public void EmptyBenchmarkRunResultsChecked()
        {
            _benchmarkRunResults.Count.Should().Be(0);
        }

        public void BenchmarkRunResultsChecked()
        {
            var results = _benchmarkRunResults.SelectMany(brr => brr.Results).ToList();

            foreach (var br in results)
            {
                br.FullName.Should().NotBeNullOrWhiteSpace();
                br.Method.Should().NotBeNullOrWhiteSpace();
                br.Namespace.Should().NotBeNullOrWhiteSpace();
            }
            _benchmarkRunResults.All(brr => brr.Results.Count > 0).Should().BeTrue();
            _benchmarkRunResults.All(brr => brr.Run != null).Should().BeTrue();
        }

        public void AnalysisArgsCreated(decimal tolerance, int maxErrors)
        {
            _analysisArgs = new AnalyseBenchmarksExecutorArgs()
            {
                Verbose = true,
                Tolerance = tolerance,
                MaxErrors = maxErrors,
                AggregatesPath = _outputAggPath,
            };
        }

        public async Task AnalysisExecutorExecuted()
        {
            var analyser = new AnalyseBenchmarksExecutor(_telemetry, new BenchmarkInfoJsonFileProvider(), new BenchmarkStatisticAccessorProvider());

            _analysisResult = await analyser.ExecuteAsync(_analysisArgs);

            _analysisResult.Should().NotBeNull();
        }

        public void AnalysisResultCheckedForSuccess()
        {
            _analysisResult.MeetsRequirements.Should().BeTrue();
            _analysisResult.InnerResults.Any(bra => bra.MeetsRequirements).Should().BeTrue();
            _analysisResult.InnerResults.Count.Should().BeGreaterThan(0);
        }

        public void AnalysisResultCheckedForFailure()
        {
            _analysisResult.MeetsRequirements.Should().BeFalse();
            _analysisResult.Message.Should().NotBeNullOrWhiteSpace();
        }

        public void ReportsDirectoryCreated()
        {
            _reportsOutputPath = IOHelper.CreateTempFolder(_workingPath, "Reports");
            
        }
        
        public async Task ReportGeneratorExecuted(ReportKind kind)
        {
            var reportArgs = new ReportGenerationArgs()
            {
                AggregatesPath = _outputAggPath,
                OutputPath = _reportsOutputPath,
                Filters = null,
            };

            var reporter = new ReporterProvider(new CsvFileWriter(), new JsonFileWriter(),
                                                new BenchmarkReader(new BenchmarkInfoJsonFileProvider()))
                .GetReporter(kind);

            _reportResult = await reporter.GenerateAsync(reportArgs);
        }

        public void CsvReportsAreVerified()
        {
            var files = _reportResult.Where(r => Path.GetExtension(r) == ".csv");

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    throw new AssertionFailedException($"File {file} does not exist.");
                }

                using (var stream = File.OpenText(file))
                {
                    using (var csvReader = new CsvHelper.CsvReader(stream, CultureInfo.InvariantCulture))
                    {
                        var recs = csvReader.GetRecords<object>().ToList();

                        recs.Count.Should().BeGreaterThan(0);
                    }
                }
            }
        }

        public void JsonReportsAreVerified()
        {
            var files = _reportResult.Where(r => Path.GetExtension(r) == ".json");

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    throw new AssertionFailedException($"File {file} does not exist.");
                }
                
                var json = File.ReadAllText(file);
                var records = JsonConvert.DeserializeObject<IList<BenchmarkRecord>>(json);
                records.Count.Should().BeGreaterThan(0);
            }
        }
    }
}

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
using Shouldly;

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
                CommitSha = Guid.NewGuid().ToString(),
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

            _aggregateResult.ShouldBe(expectedResult);
        }

        public async Task AggregatedBenchmarkInfosFetched()
        {
            var infoReader = new BenchmarkInfoJsonFileProvider();

            _newBenchmarkInfos = await infoReader.GetBenchmarkInfosAsync(_outputAggPath);
        }

        public void SingletonBenchmarkInfoIsChecked()
        {
            _newBenchmarkInfos.Count.ShouldBe(1);

            _newBenchmarkInfos[0].BranchName.ShouldBe(_aggregateArgs.BranchName);
            _newBenchmarkInfos[0].CommitSha.ShouldBe(_aggregateArgs.CommitSha);
            _newBenchmarkInfos[0].BuildUri.ShouldBe(_aggregateArgs.BuildUri);
            _newBenchmarkInfos[0].Tags.ShouldBe(_aggregateArgs.Tags);

            _newBenchmarkInfos[0].Runs.Count.ShouldBe(_newRunFiles);
            
            _newBenchmarkInfos[0].Runs.All(p => !string.IsNullOrWhiteSpace(p.BenchmarkDotNetVersion)).ShouldBeTrue();
            _newBenchmarkInfos[0].Runs.All(p => p.Creation > DateTimeOffset.MinValue).ShouldBeTrue();
        }

        public void MultipleBenchmarkInfosAreChecked()
        {
            _newBenchmarkInfos.Count.ShouldBe(_aggregateArgs.BenchmarkRuns);

            foreach (var bi in _newBenchmarkInfos)
            {
                bi.BranchName.ShouldBe(_aggregateArgs.BranchName);
                bi.CommitSha.ShouldBe(_aggregateArgs.CommitSha);
                bi.BuildUri.ShouldBe(_aggregateArgs.BuildUri);
                bi.Tags.ShouldBe(_aggregateArgs.Tags);

                bi.Runs.Count.ShouldBe(_newRunFiles);

                bi.Runs.All(p => !string.IsNullOrWhiteSpace(p.BenchmarkDotNetVersion)).ShouldBeTrue();
                bi.Runs.All(p => p.Creation > DateTimeOffset.MinValue).ShouldBeTrue();
            }
        }
        
        public void BenchmarkRunResultsRetrieved()
        {
            var rdr = new BenchmarkRunResultsReader();

            _benchmarkRunResults = rdr.GetBenchmarkResults(_newBenchmarkInfos);
        }

        public void EmptyBenchmarkRunResultsChecked()
        {
            _benchmarkRunResults.Count.ShouldBe(0);
        }

        public void BenchmarkRunResultsChecked()
        {
            var results = _benchmarkRunResults.SelectMany(brr => brr.Results).ToList();

            foreach (var br in results)
            {
                br.FullName.ShouldNotBeNullOrWhiteSpace();
                br.Method.ShouldNotBeNullOrWhiteSpace();
                br.Namespace.ShouldNotBeNullOrWhiteSpace();
            }
            _benchmarkRunResults.All(brr => brr.Results.Count > 0).ShouldBeTrue();
            _benchmarkRunResults.All(brr => brr.Run != null).ShouldBeTrue();
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

            _analysisResult.ShouldNotBeNull();
        }

        public void AnalysisResultCheckedForSuccess()
        {
            _analysisResult.MeetsRequirements.ShouldBeTrue();
            _analysisResult.InnerResults.Any(bra => bra.MeetsRequirements).ShouldBeTrue();
            _analysisResult.InnerResults.Count.ShouldBeGreaterThan(0);
        }

        public void AnalysisResultCheckedForFailure()
        {
            _analysisResult.MeetsRequirements.ShouldBeFalse();
            _analysisResult.Message.ShouldNotBeNullOrWhiteSpace();
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
                    throw new Exception($"File {file} does not exist.");
                }

                using (var stream = File.OpenText(file))
                {
                    using (var csvReader = new CsvHelper.CsvReader(stream, CultureInfo.InvariantCulture))
                    {
                        var recs = csvReader.GetRecords<object>().ToList();

                        recs.Count.ShouldBeGreaterThan(0);
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
                    throw new Exception($"File {file} does not exist.");
                }
                
                var json = File.ReadAllText(file);
                var records = JsonConvert.DeserializeObject<IList<BenchmarkRecord>>(json);
                records.Count.ShouldBeGreaterThan(0);
            }
        }
    }
}

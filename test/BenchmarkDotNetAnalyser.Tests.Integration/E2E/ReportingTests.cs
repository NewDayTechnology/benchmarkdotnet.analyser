using BenchmarkDotNetAnalyser.Reporting;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.E2E
{
    public class ReportingTests : BaseTests
    {
        
        [BddfyTheory]
        [MemberData(nameof(GetFilePaths))]
        public void Reporting_SingleRunAggregation_CsvGenerated(string filePath)
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(new[] {filePath}))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .When(s => s.ReportsDirectoryCreated())
                .And(s => s.ReportGeneratorExecuted(ReportKind.Csv))
                .Then(s => s.CsvReportsAreVerified())
                .BDDfy();
        }

        
        [BddfyFact]
        public void Reporting_MultipleRunAggregation_CsvGenerated()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(GetSourceResultFilePaths()))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .When(s => s.ReportsDirectoryCreated())
                .When(s => s.ReportGeneratorExecuted(ReportKind.Csv))
                .Then(s => s.CsvReportsAreVerified())
                .BDDfy();
        }

        
        [BddfyTheory]
        [MemberData(nameof(GetFilePaths))]
        public void Reporting_SingleRunAggregation_JsonGenerated(string filePath)
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(new[] {filePath}))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .When(s => s.ReportsDirectoryCreated())
                .And(s => s.ReportGeneratorExecuted(ReportKind.Json))
                .Then(s => s.JsonReportsAreVerified())
                .BDDfy();
        }
        
        
        [BddfyFact]
        public void Reporting_MultipleRunAggregation_JsonGenerated()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(GetSourceResultFilePaths()))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .When(s => s.ReportsDirectoryCreated())
                .When(s => s.ReportGeneratorExecuted(ReportKind.Json))
                .Then(s => s.JsonReportsAreVerified())
                .BDDfy();
        }

    }
}

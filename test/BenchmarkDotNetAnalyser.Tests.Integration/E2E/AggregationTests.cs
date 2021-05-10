using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.E2E
{
    public class AggregationTests : BaseTests
    {
        
        [BddfyFact]
        
        public void Aggregation_ZeroNewRuns_EmptyAggregationFolder_EmptyResultsExpected()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AnAggregateOutputDirectoryCreated())

                .When(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .Then(s => s.AggregatedBenchmarkInfosFetched())
                .And(s => s.SingletonBenchmarkInfoIsChecked())
                .And(s => s.BenchmarkRunResultsRetrieved())
                .And(s => s.EmptyBenchmarkRunResultsChecked())
                .BDDfy();
        }

        [BddfyTheory]
        [MemberData(nameof(GetFilePaths))]
        public void Aggregation_SingleNewRun_EmptyAggregationFolder_MultipleResultsExpected(string filePath)
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(new[] {filePath}))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AnAggregateOutputDirectoryCreated())

                .When(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .Then(s => s.AggregatedBenchmarkInfosFetched())
                .And(s => s.SingletonBenchmarkInfoIsChecked())
                .And(s => s.BenchmarkRunResultsRetrieved())
                .And(s => s.BenchmarkRunResultsChecked())
                .BDDfy();
        }

        [BddfyFact]
        
        public void Aggregation_MultipleNewRuns_EmptyAggregationFolder_MultipleResultsExpected()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(GetSourceResultFilePaths()))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AnAggregateOutputDirectoryCreated())

                .When(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .Then(s => s.AggregatedBenchmarkInfosFetched())
                .And(s => s.SingletonBenchmarkInfoIsChecked())
                .And(s => s.BenchmarkRunResultsRetrieved())
                .And(s => s.BenchmarkRunResultsChecked())
                .BDDfy();
        }

        [BddfyFact]
        public void Aggregation_ZeroNewRuns_AggregateExistingRuns_MultipleResultsExpected()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.AggregregatesDirectoryCreated())
                
                .When(s => s.AggregateArgumentsAreCreatedWithRuns(2))
                .And(s => s.AggregationExecutorExecuted(true))
                .And(s => s.AggregationExecutorExecuted(true))

                .Then(s => s.AggregatedBenchmarkInfosFetched())
                .And(s => s.MultipleBenchmarkInfosAreChecked())
                .And(s => s.BenchmarkRunResultsRetrieved())
                .And(s => s.BenchmarkRunResultsChecked())
                .BDDfy();
        }

        [BddfyTheory]
        [MemberData(nameof(GetFilePaths))]
        public void Aggregation_SingleNewRun_AggregateExistingRuns_MultipleResultsExpected(string filePath)
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(new[] {filePath}))
                .And(s => s.AggregregatesDirectoryCreated())
                
                .When(s => s.AggregateArgumentsAreCreatedWithRuns(2))
                .And(s => s.AggregationExecutorExecuted(true))
                .And(s => s.AggregationExecutorExecuted(true))

                .Then(s => s.AggregatedBenchmarkInfosFetched())
                .And(s => s.MultipleBenchmarkInfosAreChecked())
                .And(s => s.BenchmarkRunResultsRetrieved())
                .And(s => s.BenchmarkRunResultsChecked())
                .BDDfy();
        }


        [BddfyFact]
        public void Aggregation_MultipleNewRuns_AggregateExistingRuns_MultipleResultsExpected()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(GetSourceResultFilePaths()))
                .And(s => s.AggregregatesDirectoryCreated())
                
                .When(s => s.AggregateArgumentsAreCreatedWithRuns(2))
                .And(s => s.AggregationExecutorExecuted(true))
                .And(s => s.AggregationExecutorExecuted(true))

                .Then(s => s.AggregatedBenchmarkInfosFetched())
                .And(s => s.MultipleBenchmarkInfosAreChecked())
                .And(s => s.BenchmarkRunResultsRetrieved())
                .And(s => s.BenchmarkRunResultsChecked())
                .BDDfy();
        }

    }
}

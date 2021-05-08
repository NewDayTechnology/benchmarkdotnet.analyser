using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.E2E
{
    public class AnalysisTests : BaseTests
    {
        
        [BddfyFact]
        public void Analysis_EmptyAggregation_AnalysisFails()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.AggregregatesDirectoryCreated())
                
                .When(s => s.AnalysisArgsCreated(0.0m, 0))
                .And(s => s.AnalysisExecutorExecuted())

                .Then(s => s.AnalysisResultCheckedForFailure())
                .BDDfy();
        }

        [BddfyTheory]
        [MemberData(nameof(GetFilePaths))]
        public void Analysis_SingleRunAggregation_AnalysisPasses(string filePath)
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(new[] {filePath}))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AggregateArgumentsAreCreatedWithRuns(1))
                .And(s => s.AggregationExecutorExecuted(true))

                .When(s => s.AnalysisArgsCreated(0.0m, 0))
                .And(s => s.AnalysisExecutorExecuted())
                
                .Then(s => s.AnalysisResultCheckedForSuccess())
                .BDDfy();
        }

        [BddfyFact]
        public void Analysis_MultipleRunsAggregation_AnalysisPasses()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(GetSourceResultFilePaths()))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AggregateArgumentsAreCreatedWithRuns(3))
                .And(s => s.AggregationExecutorExecuted(true))
                .And(s => s.AggregationExecutorExecuted(true))
                .And(s => s.AggregationExecutorExecuted(true))

                .When(s => s.AnalysisArgsCreated(0.0m, 0))
                .And(s => s.AnalysisExecutorExecuted())

                .Then(s => s.AnalysisResultCheckedForSuccess())
                .BDDfy();
        }

        [BddfyFact]
        public void Analysis_MultipleRunsAggregation_NegativeMaxErrorCount_AnalysisFails()
        {
            new BaseStory()
                .Given(s => s.RunsDirectoryCreated())
                .And(s => s.RunDataFilesAreCopiedToRunsDirectory(GetSourceResultFilePaths()))
                .And(s => s.AggregregatesDirectoryCreated())
                .And(s => s.AggregateArgumentsAreCreatedWithRuns(3))
                .And(s => s.AggregationExecutorExecuted(true))
                .And(s => s.AggregationExecutorExecuted(true))
                .And(s => s.AggregationExecutorExecuted(true))

                .When(s => s.AnalysisArgsCreated(0.0m, -10))
                .And(s => s.AnalysisExecutorExecuted())

                .Then(s => s.AnalysisResultCheckedForFailure())
                .BDDfy();
        }
    }
}

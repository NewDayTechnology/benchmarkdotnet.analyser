namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IAggregateBenchmarksCommandValidator
    {
        void Validate(AggregateBenchmarksCommand command);
    }
}

namespace BenchmarkDotNetAnalyser.Commands
{
    public interface IAnalyseBenchmarksCommandValidator<T>
    where T : BaseAnalyseBenchmarksCommand
    {
        void Validate(T command);
    }
}

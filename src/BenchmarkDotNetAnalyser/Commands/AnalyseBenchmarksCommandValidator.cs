namespace BenchmarkDotNetAnalyser.Commands
{
    public class AnalyseBenchmarksCommandValidator : BaseAnalyseBenchmarksCommandValidator, IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>
    {
        public void Validate(AnalyseBenchmarksCommand command)
        {
            base.Validate(command.ArgNotNull(nameof(command)));

            ValidateAndFixParameters(command);
        }

        private void ValidateAndFixParameters(AnalyseBenchmarksCommand command)
        {
            command.Tolerance = command.Tolerance.PipeIfNotNull(x =>
            {
                var ok = decimal.TryParse(x, out var y);
                ok.InvalidOpArg(x => !x, $"The {command.GetCommandOptionName(nameof(command.Tolerance))} parameter must be numeric.");
                y.InvalidOpArg(y => y < decimal.Zero, $"The {command.GetCommandOptionName(nameof(command.Tolerance))} parameter must have be least zero or positive value.");
                return x;
            }, "0");

            command.MaxErrors = command.MaxErrors.PipeIfNotNull(x =>
                {
                    var ok = int.TryParse(x, out var y);
                    ok.InvalidOpArg(x => !x, $"The {command.GetCommandOptionName(nameof(command.MaxErrors))} parameter must be numeric.");
                    y.InvalidOpArg(y => y < 0, $"The {command.GetCommandOptionName(nameof(command.MaxErrors))} parameter must have be least zero or positive value.");
                    return x;
                },"0");
        }
    }
}

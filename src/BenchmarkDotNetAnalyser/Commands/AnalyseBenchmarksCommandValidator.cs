using System;
using System.Linq;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Commands
{
    public class AnalyseBenchmarksCommandValidator : BaseAnalyseBenchmarksCommandValidator, IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>
    {
        private readonly IBenchmarkStatisticAccessorProvider _accessors;

        public AnalyseBenchmarksCommandValidator(IBenchmarkStatisticAccessorProvider accessors)
        {
            _accessors = accessors.ArgNotNull(nameof(accessors));
        }

        public void Validate(AnalyseBenchmarksCommand command)
        {
            base.Validate(command.ArgNotNull(nameof(command)));

            ValidateAndFixParameters(command);
        }

        private void ValidateAndFixParameters(AnalyseBenchmarksCommand command)
        {
            var accessorInfos = _accessors.GetAccessorInfos().ToList();

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

            command.Statistic = command.Statistic.PipeIfNotNull(arg =>
            {
                var accessorNames = accessorInfos.Select(ai => ai.Name);
                var accessor = accessorNames.FirstOrDefault(s => StringComparer.InvariantCultureIgnoreCase.Equals(arg, s));

                arg.InvalidOpArg(x => accessor == null,
                    $"Invalid statistic name. Valid ones are: {accessorNames.Join(", ")}");

                return accessor;
            }, accessorInfos.FirstOrDefault(ai => ai.IsDefault)?.Name);
        }
    }
}

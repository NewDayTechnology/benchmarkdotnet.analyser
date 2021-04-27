using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace BenchmarkDotNetAnalyser.Commands
{
    [Command("version", Description = "Get the tool's version info.")]
    public class VersionCommand
    {
        private readonly IConsole _console;

        public VersionCommand(IConsole console)
        {
            _console = console.ArgNotNull(nameof(console));
        }

        public Task<int> OnExecuteAsync()
        {
            var line = ProgramBootstrap.GetDescription();
            _console.WriteLine(line);
            
            return Task.FromResult(true.ToReturnCode());
        }
    }

}

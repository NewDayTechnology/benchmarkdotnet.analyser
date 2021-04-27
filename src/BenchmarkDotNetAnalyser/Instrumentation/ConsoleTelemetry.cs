using McMaster.Extensions.CommandLineUtils;

namespace BenchmarkDotNetAnalyser.Instrumentation
{
    public class ConsoleTelemetry : ITelemetry
    {
        private readonly IConsole _console;
        private bool _verbose;

        public ConsoleTelemetry(IConsole console)
        {
            _console = console;
        }
        
        public ITelemetry SetVerbosity(bool value)
        {
            _verbose = value;
            return this;
        }

        public void Commentary(string message) => TelemetryEntry.Commentary(message, true).PipeDo(Write);
        
        public void Info(string message) =>  TelemetryEntry.Info(message, true).PipeDo(Write);

        public void Success(string message) => TelemetryEntry.Success(message, true).PipeDo(Write);

        public void Warning(string message) => TelemetryEntry.Warning(message, true).PipeDo(Write);

        public void Error(string message) => TelemetryEntry.Error(message, true).PipeDo(Write);

        public void Write(TelemetryEntry line)
        {
            if (!line.IsVerbose || _verbose)
            {
                _console.ForegroundColor = line.Color;
                if (line.AddLineBreak)
                {
                    _console.WriteLine(line.Message);
                }
                else
                {
                    _console.Write(line.Message);
                }

                _console.ResetColor();
            }
        }
    }
}

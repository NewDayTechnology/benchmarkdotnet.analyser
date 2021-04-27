namespace BenchmarkDotNetAnalyser.Instrumentation
{
    public interface ITelemetry
    {
        ITelemetry SetVerbosity(bool value);
        void Commentary(string message);
        void Info(string message);
        void Success(string message);
        void Warning(string message);
        void Error(string message);
        void Write(TelemetryEntry entry);
    }
}

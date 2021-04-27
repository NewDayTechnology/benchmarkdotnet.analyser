using System;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Instrumentation
{
    internal static class TelemetryExtensions
    {
        public static async Task<TResult> InvokeWithLoggingAsync<TResult>(this ITelemetry telemetry, TelemetryEntry log,
            Func<Task<TResult>> func)
        {
            telemetry.ArgNotNull(nameof(telemetry));
            func.ArgNotNull(nameof(func));

            try
            {
                telemetry.Write(log);

                var result = await func();

                EmitDoneEntry(telemetry, log);

                return result;
            }
            catch 
            {
                EmitErrorEntry(telemetry, log);

                throw;
            }
        }

        public static TResult InvokeWithLogging<TResult>(this ITelemetry telemetry, TelemetryEntry log,  Func<TResult> func)
        {
            try
            {
                telemetry.Write(log);

                var result = func();

                EmitDoneEntry(telemetry, log);

                return result;
            }
            catch
            {
                EmitErrorEntry(telemetry, log);

                throw;
            }
        }

        private static void EmitErrorEntry(ITelemetry telemetry, TelemetryEntry log)
        {
            if (!log.AddLineBreak)
            {
                telemetry.Write(TelemetryEntry.Error("", true));
            }
        }

        private static void EmitDoneEntry(ITelemetry telemetry, TelemetryEntry log)
        {
            var newLog = new TelemetryEntry(log.Color, true, "done.")
            {
                IsVerbose = log.IsVerbose
            };

            telemetry.Write(newLog);
        }
    }
}

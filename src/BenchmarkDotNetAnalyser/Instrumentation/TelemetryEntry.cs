using System;
using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Instrumentation
{
    [ExcludeFromCodeCoverage]
    public class TelemetryEntry
    {
        internal TelemetryEntry(ConsoleColor color, string message)
        {
            Color = color;
            Message = message;
        }

        internal TelemetryEntry(ConsoleColor color, bool addLineBreak, string message)
        {
            Color = color;
            AddLineBreak = addLineBreak;
            Message = message;
        }

        public ConsoleColor Color { get; set; }
        public bool AddLineBreak { get; set; }
        public string Message { get; set; }
        public bool IsVerbose { get; set; }

        public static TelemetryEntry Commentary(string message) => Commentary(message, false);

        public static TelemetryEntry Commentary(string message, bool addLineBreak) => new TelemetryEntry(ConsoleColor.DarkGray, addLineBreak, message){ IsVerbose = true};

        public static TelemetryEntry Info(string message, bool addLineBreak) => new TelemetryEntry(ConsoleColor.White, addLineBreak, message);

        public static TelemetryEntry Success(string message, bool addLineBreak) => new TelemetryEntry(ConsoleColor.Green, addLineBreak, message);

        public static TelemetryEntry Warning(string message, bool addLineBreak) => new TelemetryEntry(ConsoleColor.Yellow, addLineBreak, message);

        public static TelemetryEntry Error(string message, bool addLineBreak, bool isVerbose = false) =>
            new TelemetryEntry(ConsoleColor.Red, addLineBreak, message)
            {
                IsVerbose = isVerbose
            };
    }

}

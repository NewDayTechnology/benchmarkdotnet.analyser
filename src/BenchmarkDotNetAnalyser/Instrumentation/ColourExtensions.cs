using System;
using Crayon;

namespace BenchmarkDotNetAnalyser.Instrumentation
{
    internal static class ColourExtensions
    {
        public static string Colourise(this string value, ConsoleColor colour)
        {
            if (value == null) return value;
            
            return (colour switch
            {
                ConsoleColor.Blue => Output.Bright.Blue(value),
                ConsoleColor.Red => Output.Bright.Red(value),
                ConsoleColor.Yellow => Output.Bright.Yellow(value),
                ConsoleColor.Green => Output.Bright.Green(value),
                ConsoleColor.Cyan => Output.Bright.Cyan(value),
                ConsoleColor.Magenta => Output.Bright.Magenta(value),
                ConsoleColor.White => Output.Bright.White(value),
                ConsoleColor.DarkBlue => Output.Dim(Output.Blue(value)),
                ConsoleColor.DarkRed => Output.Dim(Output.Red(value)),
                ConsoleColor.DarkYellow => Output.Dim(Output.Yellow(value)),
                ConsoleColor.DarkGreen => Output.Dim(Output.Green(value)),
                ConsoleColor.DarkCyan => Output.Dim(Output.Cyan(value)),
                ConsoleColor.DarkMagenta => Output.Dim(Output.Magenta(value)),
                _ => Output.Dim(value),
            });
        }
    }
}

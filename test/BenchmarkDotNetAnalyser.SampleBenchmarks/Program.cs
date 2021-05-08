using System;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNetAnalyser.SampleBenchmarks
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                    .Run(args);
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return 1;
            }
        }
    }
}

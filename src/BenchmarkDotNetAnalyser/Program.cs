using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BenchmarkDotNetAnalyser.Commands;
using McMaster.Extensions.CommandLineUtils;

[assembly: InternalsVisibleTo("BenchmarkDotNetAnalyser.Tests.Unit")]

namespace BenchmarkDotNetAnalyser
{
    [ExcludeFromCodeCoverage]
    [Subcommand(typeof(AggregateBenchmarksCommand))]
    [Subcommand(typeof(AnalyseBenchmarksCommand))]
    [Subcommand(typeof(VersionCommand))]
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(ProgramBootstrap.CreateServiceCollection());
            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return false.ToReturnCode();
            }
        }

        
        private int OnExecute(CommandLineApplication app)
        {
            app.Description = ProgramBootstrap.GetDescription();
            app.ShowHelp();
            return true.ToReturnCode();
        }

        
    }
}

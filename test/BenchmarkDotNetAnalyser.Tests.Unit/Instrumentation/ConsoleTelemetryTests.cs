using System.IO;
using BenchmarkDotNetAnalyser.Instrumentation;
using FsCheck;
using FsCheck.Xunit;
using McMaster.Extensions.CommandLineUtils;
using NSubstitute;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Instrumentation
{
    public class ConsoleTelemetryTests
    {
        [Property(Verbose = true)]
        public bool Commentary_Verbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(true);

            telemetry.Commentary(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        [Property(Verbose = true)]
        public bool Commentary_NonVerbose_MessageNotWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(false);

            telemetry.Commentary(message.Get);
            
            writer.Received(0).WriteLine(message.Get);

            return true;
        }

        
        [Property(Verbose = true)]
        public bool Info_Verbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(true);

            telemetry.Info(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        [Property(Verbose = true)]
        public bool Info_NonVerbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(false);

            telemetry.Info(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        [Property(Verbose = true)]
        public bool Success_Verbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(true);

            telemetry.Success(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        [Property(Verbose = true)]
        public bool Success_NonVerbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(false);

            telemetry.Success(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        
        [Property(Verbose = true)]
        public bool Warning_Verbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(true);

            telemetry.Warning(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        [Property(Verbose = true)]
        public bool Warning_NonVerbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(false);

            telemetry.Warning(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }


        
        [Property(Verbose = true)]
        public bool Error_Verbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(true);

            telemetry.Error(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        [Property(Verbose = true)]
        public bool Error_NonVerbose_MessageWritten(NonEmptyString message)
        {
            var console = CreateMockConsole(out var writer);

            var telemetry = new ConsoleTelemetry(console).SetVerbosity(false);

            telemetry.Error(message.Get);
            
            writer.Received(1).WriteLine(Arg.Is<string>(s => s.Contains(message.Get)));

            return true;
        }

        private static IConsole CreateMockConsole(out TextWriter writer)
        {
            var console = Substitute.For<IConsole>();
            writer = Substitute.For<TextWriter>();
            console.Out.Returns(writer);
            return console;
        }
    }
}

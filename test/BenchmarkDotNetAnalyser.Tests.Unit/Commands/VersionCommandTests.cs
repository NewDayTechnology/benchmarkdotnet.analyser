using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Commands;
using Shouldly;
using McMaster.Extensions.CommandLineUtils;
using NSubstitute;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class VersionCommandTests
    {
        [Fact]
        public async Task OnExecuteAsync_OkReturned()
        {
            var console = Substitute.For<IConsole>();
            var writer = Substitute.For<System.IO.TextWriter>();
            console.Out.Returns(writer);

            var cmd = new VersionCommand(console);

            var result = await cmd.OnExecuteAsync();

            result.ShouldBe(0);
            console.WriteLine(Arg.Any<string>());
            writer.Received(1).WriteLine(Arg.Any<string>());
        }
    }
}

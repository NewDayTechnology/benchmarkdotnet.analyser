using System.Linq;
using BenchmarkDotNetAnalyser.IO;
using FluentAssertions;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Integration.Classes.IO
{
    public class FileFinderTests
    {
        [Fact]
        public void Find_ReturnsFiles()
        {
            var ext = ".json";
            var root = IOHelper.GetSourceFilePath();
            var finder = new FileFinder();

            var result = finder.Find(root, $"*{ext}").ToList();

            var expected = IOHelper.GetSourceResultFilePaths().ToList();

            result.Count.Should().BeGreaterThan(0);
            result.SequenceEqual(expected).Should().BeTrue();
            result.All(x => x.EndsWith(ext)).Should().BeTrue();
        }
    }
}

using System.Collections.Generic;

namespace BenchmarkDotNetAnalyser.IO
{
    public interface IFileFinder
    {
        IEnumerable<string> Find(string root, string suffix);
    }
}

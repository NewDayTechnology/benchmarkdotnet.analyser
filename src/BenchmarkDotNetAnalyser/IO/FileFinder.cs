using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace BenchmarkDotNetAnalyser.IO
{
    [ExcludeFromCodeCoverage]
    public class FileFinder : IFileFinder
    {
        public IEnumerable<string> Find(string root, string suffix)
        {
            root.ArgNotNull(nameof(root));
            suffix.ArgNotNull(nameof(suffix));

            return new DirectoryInfo(root)
                        .EnumerateFiles($"*{suffix}", SearchOption.AllDirectories)
                        .Where(fi => fi.Length > 0)
                        .Select(fi => fi.FullName);
        }
    }
}

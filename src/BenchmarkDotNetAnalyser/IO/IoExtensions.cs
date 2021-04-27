using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace BenchmarkDotNetAnalyser.IO
{
    [ExcludeFromCodeCoverage]
    internal static class IoExtensions
    {
        public static string AssertPathExists(this string path) =>
            path.ArgNotNull(nameof(path))
                .InvalidOpArg(p => !Directory.Exists(p), $"The directory '{path}' does not exist.");

        public static string ResolveWorkingPath(this string path)
        {
            path.ArgNotNull(nameof(path));
            
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            var workingPath = Directory.GetCurrentDirectory();

            return Path.Combine(workingPath, path);
        }

        public static string GetOrCreateFullPath(this string path)
        {
            path.ArgNotNull(nameof(path));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}

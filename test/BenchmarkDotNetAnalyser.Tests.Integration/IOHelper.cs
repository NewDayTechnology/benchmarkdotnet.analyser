using System;
using System.Collections.Generic;
using System.IO;

namespace BenchmarkDotNetAnalyser.Tests.Integration
{
    internal static class IOHelper
    {
        public static IEnumerable<string> GetSourceResultFilePaths()
        {
            var sourcePath = GetSourceFilePath();

            return Directory.EnumerateFiles(sourcePath, "*-report-full.json");
        }

        public static string GetSourceFilePath()
        {
            var curDir = Directory.GetCurrentDirectory();
            var sourcePath = Path.Combine(curDir, "BenchmarkDotNetResults");

            return sourcePath;
        }

        public static string CreateTempFolder()
        {
            var curDir = Directory.GetCurrentDirectory();
            
            var testFolder = $"{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}-{Guid.NewGuid()}";

            var path = Path.Combine(curDir, "IntegrationTestsResults", testFolder);

            return Directory.CreateDirectory(path).FullName;
        }

        public static string CreateTempFolder(string root, string dir)
        {
            var path = Path.Combine(root, dir);

            return Directory.CreateDirectory(path).FullName;
        }

        public static void CopyFiles(string targetDir, IEnumerable<string> sourceFiles)
        {
            foreach (var sourceFile in sourceFiles)
            {
                var fileName = Path.GetFileName(sourceFile);
                var destFile = Path.Combine(targetDir, fileName);

                File.Copy(sourceFile, destFile);
            }
        }
    }
}

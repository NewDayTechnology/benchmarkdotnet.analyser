using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkInfoJsonFileProvider : BaseBenchmarkInfoJsonProvider
    {
        private const string ManifestFileName = "aggregatebenchmarks.manifest.json";

        protected override async Task<string> GetBenchmarkManifestJsonAsync(string path)
        {
            var filePath = GetBenchmarksManifestPath(path);

            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }
            return null;
        }

        protected override Task WriteBenchmarkManifestJsonAsync(string path, string json)
        {
            var filePath = GetBenchmarksManifestPath(path);

            return File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }

        protected override async Task CopyBenchmarkFilesAsync(string sourcePath, string destinationPath,
            IEnumerable<BenchmarkInfo> benchmarkInfos)
        {
            var newFolders = await Task.FromResult(CopyBenchmarkFilesInner(sourcePath, destinationPath, benchmarkInfos));

            CleanupOutputPath(destinationPath, newFolders);
        }

        private void CleanupOutputPath(string destinationPath, IEnumerable<string> newFolders)
        {
            var preserved = newFolders.ToHashSet();
            var children = new DirectoryInfo(destinationPath).GetDirectories();
            var victims = children.Where(di => !preserved.Contains(di.Name)).Select(di => di.FullName);

            foreach (var victimPath in victims)
            {
                try
                {
                    Directory.Delete(victimPath, true);
                }
                catch (IOException)
                {
                } 
            }
        }

        private IList<string> CopyBenchmarkFilesInner(string sourcePath, string destinationRootPath,
            IEnumerable<BenchmarkInfo> benchmarkInfos)
        {
            var result = new List<string>();
            var destinationFolderCache = new Dictionary<string, string>();

            foreach (var bi in benchmarkInfos)
            {
                foreach (var bri in bi.Runs.NullToEmpty())
                {
                    // set the destination folder for this file
                    var cacheKey = Path.GetDirectoryName(bri.FullPath);
                    if (!destinationFolderCache.TryGetValue(cacheKey, out var destinationRelativeFolder))
                    {
                        destinationRelativeFolder = Path.IsPathRooted(bri.FullPath)
                            ? Guid.NewGuid().ToString()
                            : Path.GetDirectoryName(bri.FullPath);
                        destinationFolderCache[cacheKey] = destinationRelativeFolder;

                        var destinationPath = Path.Combine(destinationRootPath, destinationRelativeFolder);
                        Directory.CreateDirectory(destinationPath);
                    }
                    
                    // set the destination file path
                    var destinationRelativePath = Path.Combine(destinationRelativeFolder, Path.GetFileName(bri.FullPath));
                    var fullDestinationPath = Path.Combine(destinationRootPath, destinationRelativePath);
                    
                    // set the source file path
                    var fullSourcePath = Path.IsPathRooted(bri.FullPath)
                        ? bri.FullPath
                        : Path.Combine(sourcePath, bri.FullPath);
                    
                    if (fullSourcePath != fullDestinationPath)
                    {
                        File.Copy(fullSourcePath, fullDestinationPath, true);
                    }

                    bri.FullPath = destinationRelativePath;

                    result.Add(destinationRelativeFolder);
                }
            }

            return result;
        }

        
        private string GetBenchmarksManifestPath(string path) => Path.Combine(path, ManifestFileName);
    }
}

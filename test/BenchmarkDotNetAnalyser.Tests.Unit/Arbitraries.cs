using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public static class AlphanumericStringArbitrary
    {
        public static Arbitrary<string> GetArbitrary() => ArbMap.Default.ArbFor<string>().Generator
            .Where(s => !string.IsNullOrEmpty(s))
            .Where(s => s.All(c => char.IsDigit(c) || char.IsLetter(c)))
            .ToArbitrary();
    }

    public static class NonIntegerStringArbitrary
    {
        public static Arbitrary<string> GetArbitrary() => ArbMap.Default.ArbFor<string>().Generator
            .Where(s => !string.IsNullOrEmpty(s))
            .Where(s => !int.TryParse(s, out _))
            .ToArbitrary();
    }

    public static class NonDecimalStringArbitrary
    {
        public static Arbitrary<string> GetArbitrary() => ArbMap.Default.ArbFor<string>().Generator
            .Where(s => !string.IsNullOrEmpty(s))
            .Where(s => !decimal.TryParse(s, out _))
            .ToArbitrary();
    }

    public static class ScannableDirectoriesArbitrary
    {
        public static Arbitrary<DirectoryInfo> GetArbitrary()
        {
            var root = Path.GetPathRoot(Directory.GetCurrentDirectory());

            var di = new DirectoryInfo(root);
            
            var rootDirs = di.EnumerateDirectories("*.", SearchOption.TopDirectoryOnly).ToList();
            var children = rootDirs.SelectMany(di =>
            {
                try
                {
                    return di.EnumerateDirectories("*.", SearchOption.TopDirectoryOnly).ToList();
                }
                catch (Exception)
                {
                    return new List<DirectoryInfo>();
                }
            });
            
            var allDirs = rootDirs.Concat(children).ToList();

            return Gen.Choose(0, allDirs.Count - 1).Select(i => allDirs[i]).ToArbitrary();
        }
    }

    public static class InvalidPathArbitrary
    {
        public static Arbitrary<string> GetArbitrary()
        {
            var disallowed = new HashSet<string>() { ".", "/", @"\"};
            return ArbMap.Default.ArbFor<string>().Generator
                .Where(s => !string.IsNullOrEmpty(s))
                .Where(s => !disallowed.Contains(s) &&
                            !s.All(c => char.IsDigit(c) || char.IsLetter(c)))
                .ToArbitrary();
        }
    }

    public static class EnumerationArbitrary<T>
        where T : struct
    {
        public static Arbitrary<T> GetArbitrary()
        {
            var values = Enum.GetValues(typeof(T)).OfType<T>().ToList();

            return Gen.Choose(0, values.Count - 1).Select(i => values[i]).ToArbitrary();
        }
    }
}

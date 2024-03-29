﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FsCheck;

namespace BenchmarkDotNetAnalyser.Tests.Unit
{
    public static class AlphanumericStringArbitrary
    {
        public static Arbitrary<string> GetArbitrary() => Arb.Default.NonEmptyString().Generator
            .Where(s => s.Get.All(c => char.IsDigit(c) || char.IsLetter(c)))
            .Select(s => s.Get)
            .ToArbitrary();
    }

    public static class NonIntegerStringArbitrary
    {
        public static Arbitrary<string> GetArbitrary() => Arb.Default.NonEmptyString().Generator
            .Where(s => !int.TryParse(s.Get, out _))
            .Select(s => s.Get)
            .ToArbitrary();
    }

    public static class NonDecimalStringArbitrary
    {
        public static Arbitrary<string> GetArbitrary() => Arb.Default.NonEmptyString().Generator
            .Where(s => !decimal.TryParse(s.Get, out _))
            .Select(s => s.Get)
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
            return Arb.Default.NonEmptyString().Generator
                .Where(s => !disallowed.Contains(s.Get) &&
                            !s.Get.All(c => char.IsDigit(c) || char.IsLetter(c)))
                .Select(s => s.Get)
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

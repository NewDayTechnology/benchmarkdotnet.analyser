using System;
using BenchmarkDotNet.Attributes;
using Force.Crc32;

namespace BenchmarkDotNetAnalyser.SampleBenchmarks.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
    [JsonExporterAttribute.Full, HtmlExporter]
    [GcServer(true)]
    [IterationCount(3)]
    public class Crc32Benchmark
    {
        private byte[] _data;
        private Random _rng;
        
        [Params(256, 512, 1024, 2048, 4096, 8192, 16384, 32768)]
        public int Size { get; set; }
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _rng = new Random();
            _data = new byte[Size];
            _rng.NextBytes(_data);
        }

        [Benchmark]
        public uint Crc32() => Crc32Algorithm.Compute(_data);
    }
}

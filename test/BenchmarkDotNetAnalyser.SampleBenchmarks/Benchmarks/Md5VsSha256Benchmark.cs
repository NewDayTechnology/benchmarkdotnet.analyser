using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNetAnalyser.SampleBenchmarks.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
    [JsonExporterAttribute.Full, HtmlExporter]
    [GcServer(true)]
    [IterationCount(3)]
    public class Md5VsSha256Benchmark
    {
        private readonly SHA256 _sha256 = SHA256.Create();
        private readonly MD5 _md5 = MD5.Create();
        private byte[] _data;

        [Params(1000, 10000, 100000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _data = new byte[N];
            new Random(42).NextBytes(_data);
        }

        [Benchmark]
        public byte[] Sha256() => _sha256.ComputeHash(_data);

        [Benchmark]
        public byte[] Md5() => _md5.ComputeHash(_data);
    }
}

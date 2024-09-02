using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNetAnalyser.SampleBenchmarks.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column]
    [JsonExporterAttribute.Full, HtmlExporter]
    [GcServer(true)]
    [IterationCount(3)]

    public class BaselinedBenchmark
    {

        [Benchmark(Baseline = true)]
        public void DoNothingBaseline()
        {

        }

        [Benchmark]
        public double DoNothing() => 2.141 * 1000000000.0;
    }
}

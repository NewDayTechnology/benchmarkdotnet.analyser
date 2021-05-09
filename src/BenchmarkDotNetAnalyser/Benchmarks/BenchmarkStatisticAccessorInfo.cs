namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public class BenchmarkStatisticAccessorInfo
    {
        internal BenchmarkStatisticAccessorInfo(string name, bool isDefault)
        {
            Name = name;
            IsDefault = isDefault;
        }

        public string Name { get; }
        public bool IsDefault { get; }
    }
}

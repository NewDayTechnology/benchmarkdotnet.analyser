using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkEnvironment
    {
        public string BenchmarkDotNetVersion { get; set; }
        public string OsVersion { get; set; }
        public string ProcessorName { get; set; }
        public int PhysicalProcessorCount { get; set; }
        public int LogicalCoreCount { get; set; }
        public string DotNetRuntimeVersion { get; set; }
        public string DotNetCliVersion { get; set; }
        public string MachineArchitecture { get; set; }
    }
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Commands
{
    [ExcludeFromCodeCoverage]
    public class AnalyseBenchmarksExecutorArgs
    {
        public string AggregatesPath { get; set; }
        public decimal Tolerance { get; set; }
        public int MaxErrors { get; set; }
        public string Statistic { get; set; }
        public IList<string> Filters { get; set; }
        public bool Verbose { get; set; }
    }
}

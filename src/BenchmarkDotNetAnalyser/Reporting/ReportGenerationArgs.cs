using System.Collections.Generic;

namespace BenchmarkDotNetAnalyser.Reporting
{
    public class ReportGenerationArgs
    {
        public string AggregatesPath { get; set; }

        public string OutputPath { get; set; }

        public IList<string> Filters { get; set; }
    }
}

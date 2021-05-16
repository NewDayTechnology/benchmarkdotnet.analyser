using System.Collections.Generic;

namespace BenchmarkDotNetAnalyser.Benchmarks
{
    public class BenchmarkRecord
    {
        public string FullName { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
        public string Parameters { get; set; }
        public IList<BenchmarkRecordCell> Cells { get; set; }
    }
}


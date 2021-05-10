using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetAnalyser.Analysis
{
    [ExcludeFromCodeCoverage]
    public class BenchmarkResultAnalysis
    {

        public string BenchmarkName { get; set; }
        public bool MeetsRequirements { get; set; }
        public string Message { get; set; }
        public IList<BenchmarkResultAnalysis> InnerResults { get; set; }
    }
}

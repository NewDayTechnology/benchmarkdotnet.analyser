using System.Collections.Generic;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.Reporting
{
    public interface IBenchmarksReportGenerator
    {
        Task<IList<string>> GenerateAsync(ReportGenerationArgs args);
    }
}

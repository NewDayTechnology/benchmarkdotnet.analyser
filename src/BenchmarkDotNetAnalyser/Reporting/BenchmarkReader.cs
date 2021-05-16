using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNetAnalyser.Benchmarks;

namespace BenchmarkDotNetAnalyser.Reporting
{
    public class BenchmarkReader : IBenchmarkReader
    {
        private readonly IBenchmarkInfoProvider _infoProvider;
        
        public BenchmarkReader(IBenchmarkInfoProvider infoProvider)
        {
            _infoProvider = infoProvider.ArgNotNull(nameof(infoProvider));
        }
        
        public async Task<IEnumerable<BenchmarkInfo>> GetBenchmarkAsync(string path, IList<string> filters)
        {
            var infos = await _infoProvider.GetBenchmarkInfosAsync(path);

            return infos
                .NullToEmpty()
                .Where(bi => bi != null)
                .Select(bi => bi.TrimRunsByFilter(filters))
                .Where(bi => bi.Runs?.Count > 0);
        }
    }
}

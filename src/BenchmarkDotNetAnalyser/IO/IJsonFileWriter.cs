using System.Collections.Generic;
using System.Threading.Tasks;

namespace BenchmarkDotNetAnalyser.IO
{
    public interface IJsonFileWriter
    {
        Task WriteAsync<T>(IEnumerable<T> rows, string filePath);
    }
}

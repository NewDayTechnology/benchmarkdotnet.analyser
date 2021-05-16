using System.Collections.Generic;

namespace BenchmarkDotNetAnalyser.IO
{
    public interface ICsvFileWriter
    {
        void Write<T>(IEnumerable<T> rows, string filePath);
    }
}
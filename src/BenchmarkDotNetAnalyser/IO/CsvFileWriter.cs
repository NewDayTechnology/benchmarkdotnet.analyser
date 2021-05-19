using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace BenchmarkDotNetAnalyser.IO
{
    [ExcludeFromCodeCoverage]
    public class CsvFileWriter : ICsvFileWriter
    {
        private readonly CsvConfiguration _config;

        public CsvFileWriter()
            : this(CreateCsvConfig())
        {
        }

        public CsvFileWriter(CsvConfiguration config)
        {
            _config = config.ArgNotNull(nameof(config));
        }

        public void Write<T>(IEnumerable<T> rows, string filePath)
        {
            rows.ArgNotNull(nameof(rows));
            filePath.ArgNotNull(nameof(filePath));

            var path = Path.GetDirectoryName(filePath).GetOrCreateFullPath();

            using var writer = new StreamWriter(filePath);
            using var csvWriter = new CsvWriter(writer, _config);

            csvWriter.WriteRecords(rows);

            csvWriter.Flush();
        }

        private static CsvConfiguration CreateCsvConfig()
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = false,
                Delimiter = ",",
                Encoding = Encoding.UTF8,
                IncludePrivateMembers = false,
                LeaveOpen = false,
                SanitizeForInjection = true
            };
        }
    }
}

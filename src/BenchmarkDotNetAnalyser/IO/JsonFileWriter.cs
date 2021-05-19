using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BenchmarkDotNetAnalyser.IO
{
    [ExcludeFromCodeCoverage]
    public class JsonFileWriter : IJsonFileWriter
    {
        private readonly JsonSerializerSettings _settings;

        public JsonFileWriter()
        {
            _settings = new JsonSerializerSettings()
            {
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                }
            };
        }

        public async Task WriteAsync<T>(IEnumerable<T> rows, string filePath)
        {
            rows.ArgNotNull(nameof(rows));
            filePath.ArgNotNull(nameof(filePath));

            var json = JsonConvert.SerializeObject(rows, _settings);

            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }
    }
}

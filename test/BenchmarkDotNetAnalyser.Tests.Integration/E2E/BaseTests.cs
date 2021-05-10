using System;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy.Configuration;
using TestStack.BDDfy.Reporters.Html;

namespace BenchmarkDotNetAnalyser.Tests.Integration.E2E
{
    public abstract class BaseTests
    {
        private static readonly Lazy<bool> ReportsConfigured = new Lazy<bool>(SetupReports);

        protected BaseTests()
        {
            var _ = ReportsConfigured.Value;
        }

        public static IList<string> GetSourceResultFilePaths() => IOHelper.GetSourceResultFilePaths().ToList();

        public static IEnumerable<object[]> GetFilePaths() => GetSourceResultFilePaths().Select(x => new[] {x});

        private static bool SetupReports()
        {
            Configurator.BatchProcessors.HtmlReport.Disable();
            
            var config = new DefaultHtmlReportConfiguration()
            {
                ReportHeader = "Benchmarkdotnet analyser",
                ReportDescription = "Integration tests",
            };

            Configurator.BatchProcessors.Add(new HtmlReporter(config, new MetroReportBuilder()));

            return true;
        }
    }
}

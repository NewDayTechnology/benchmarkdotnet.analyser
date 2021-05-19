using System;
using BenchmarkDotNetAnalyser.IO;
using BenchmarkDotNetAnalyser.Reporting;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Reporting
{
    public class ReporterProviderTests
    {
        [Property(Verbose = true, Arbitrary = new [] { typeof(EnumerationArbitrary<ReportKind>) })]
        public bool GetReporter_ByString_KnownKind_ReturnsReporter(ReportKind kind)
        {
            var k = kind.ToString();

            var reporter = CreateMockProvider().GetReporter(k);

            return reporter != null;
        }

        
        [Property(Verbose = true)]
        public bool GetReporter_ByString_UnknownKind_ThrowsException(NegativeInt kind)
        {
            var reportKind = kind.Get.ToString();

            var result = CreateMockProvider().GetReporter(reportKind);

            return result != null;
        }
        
        [Property(Verbose = true, Arbitrary = new [] { typeof(EnumerationArbitrary<ReportKind>) })]
        public bool GetReporter_KnownKind_ReturnsReporter(ReportKind kind)
        {
            var reporter = CreateMockProvider().GetReporter(kind);

            return reporter != null;
        }

        [Property(Verbose = true)]
        public bool GetReporter_UnknownKind_ThrowsException(NegativeInt kind)
        {
            try
            {
                CreateMockProvider().GetReporter((ReportKind)kind.Get);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                return !string.IsNullOrWhiteSpace(ex.Message);
            }
        }

        private static ReporterProvider CreateMockProvider() =>
            new ReporterProvider(Substitute.For<ICsvFileWriter>(), Substitute.For<IJsonFileWriter>(),
                                 Substitute.For<IBenchmarkReader>());
    }
}

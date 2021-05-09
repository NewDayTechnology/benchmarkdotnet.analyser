using System;
using System.Globalization;
using System.IO;
using System.Linq;
using BenchmarkDotNetAnalyser.Analysis;
using BenchmarkDotNetAnalyser.Benchmarks;
using BenchmarkDotNetAnalyser.Commands;
using BenchmarkDotNetAnalyser.Instrumentation;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class AnalyseBenchmarksCommandValidatorTests
    {
        [Property(Verbose = true, Arbitrary = new[] { typeof(InvalidPathArbitrary)} )]
        public bool Validate_AggregatesPath_InvalidPath(string path)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(), Substitute.For<IBenchmarkInfoProvider>(), 
                                                    Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(),
                                                    Substitute.For<IAnalyseBenchmarksExecutor>(), 
                                                    Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());
            try
            {
                validator.Validate(cmd);

                return false;
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }

        
        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary)})]
        public bool Validate_AggregatesPath_ValidPath(DirectoryInfo path)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(), Substitute.For<IBenchmarkInfoProvider>(), 
                                                   Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                                                   Substitute.For<IAnalyseBenchmarksExecutor>(),
                                                   Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            validator.Validate(cmd);

            return true;
        }

        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary)})]
        public bool Validate_MaxErrors_Default(DirectoryInfo path)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(),
                Substitute.For<IBenchmarkInfoProvider>(),
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                MaxErrors = null,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            validator.Validate(cmd);

            return cmd.MaxErrors == 0.ToString(CultureInfo.InvariantCulture);
        }

        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary)})]
        public bool Validate_MaxErrors_ValidForm(NonNegativeInt value, DirectoryInfo path)
        {
            var maxErrors = value.Get.ToString(CultureInfo.InvariantCulture);
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(), Substitute.For<IBenchmarkInfoProvider>(), 
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                MaxErrors = maxErrors,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            validator.Validate(cmd);

            return cmd.MaxErrors == maxErrors;
        }

        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary), typeof(NonIntegerStringArbitrary) })]
        public bool Validate_MaxErrors_InvalidForm(DirectoryInfo path, string maxErrors)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(),
                Substitute.For<IBenchmarkInfoProvider>(),
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                MaxErrors = maxErrors,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            try
            {
                validator.Validate(cmd);

                return false;
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.Should().NotBeNullOrWhiteSpace();
                return true;
            }
        }

        
        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary) })]
        public bool Validate_MaxErrors_NegativeValue(DirectoryInfo path, NegativeInt maxErrors)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(),
                Substitute.For<IBenchmarkInfoProvider>(),
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                MaxErrors = maxErrors.Get.ToString(),
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            try
            {
                validator.Validate(cmd);

                return false;
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.Should().NotBeNullOrWhiteSpace();
                return true;
            }
        }

        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary) })]
        public bool Validate_Tolerance_Default(DirectoryInfo path)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(), Substitute.For<IBenchmarkInfoProvider>(), 
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                Tolerance = null,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            validator.Validate(cmd);

            return cmd.Tolerance == 0.ToString(CultureInfo.InvariantCulture);
        }
        
        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary) })]
        public bool Validate_Tolerance_ValidForm(NonNegativeInt value, DirectoryInfo path)
        {
            var tolerance = value.Get.ToString(CultureInfo.InvariantCulture);
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(), Substitute.For<IBenchmarkInfoProvider>(), 
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(), Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                Tolerance = tolerance,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            validator.Validate(cmd);

            return cmd.Tolerance == tolerance;
        }
        
        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary), typeof(NonDecimalStringArbitrary) })]
        public bool Validate_Tolerance_InvalidForm(DirectoryInfo path, string tolerance)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(), Substitute.For<IBenchmarkInfoProvider>(), 
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(), Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                Tolerance = tolerance,
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            try
            {
                validator.Validate(cmd);

                return false;
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.Should().NotBeNullOrWhiteSpace();
                return true;
            }
        }

        [Property(Verbose = true, Arbitrary = new [] { typeof(ScannableDirectoriesArbitrary) })]
        public bool Validate_Tolerance_NegativeValue(DirectoryInfo path, NegativeInt tolerance)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(),
                Substitute.For<IBenchmarkInfoProvider>(),
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                Tolerance = tolerance.Get.ToString(),
            };

            var validator = new AnalyseBenchmarksCommandValidator(Substitute.For<IBenchmarkStatisticAccessorProvider>());

            try
            {
                validator.Validate(cmd);

                return false;
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.Should().NotBeNullOrWhiteSpace();
                return true;
            }
        }

        [Property(Verbose = true, Arbitrary = new[] {typeof(ScannableDirectoriesArbitrary)})]
        public bool Validate_Statistic_UnknownStatistic_ExceptionThrown(DirectoryInfo path)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(),
                Substitute.For<IBenchmarkInfoProvider>(),
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                Tolerance = "0",
                Statistic = ""
            };

            var accessorProvider = Substitute.For<IBenchmarkStatisticAccessorProvider>();
            accessorProvider.GetAccessorInfos().Returns(_ => Enumerable.Empty<BenchmarkStatisticAccessorInfo>());
            
            var validator = new AnalyseBenchmarksCommandValidator(accessorProvider);
            
            try
            {
                validator.Validate(cmd);

                return false;
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.Should().NotBeNullOrWhiteSpace();
                return true;
            }
        }

        [Property(Verbose = true, Arbitrary = new[] {typeof(ScannableDirectoriesArbitrary)})]
        public bool Validate_Statistic_KnownStatistic_Passes(DirectoryInfo path, NonEmptyString name)
        {
            var cmd = new AnalyseBenchmarksCommand(Substitute.For<ITelemetry>(),
                Substitute.For<IBenchmarkInfoProvider>(),
                Substitute.For<IAnalyseBenchmarksCommandValidator<AnalyseBenchmarksCommand>>(), 
                Substitute.For<IAnalyseBenchmarksExecutor>(),
                Substitute.For<IBenchmarkResultAnalysisReporter>())
            {
                AggregatesPath = path.FullName,
                Tolerance = "0",
                Statistic = name.Get,
            };

            var accessorProvider = Substitute.For<IBenchmarkStatisticAccessorProvider>();
            accessorProvider.GetAccessorInfos().Returns(_ => new [] { new BenchmarkStatisticAccessorInfo(name.Get, false) } );
            
            var validator = new AnalyseBenchmarksCommandValidator(accessorProvider);
            
            validator.Validate(cmd);

            return true;
        }
    }
}


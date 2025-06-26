using System;
using System.ComponentModel;
using BenchmarkDotNetAnalyser.Commands;
using Shouldly;
using McMaster.Extensions.CommandLineUtils;
using Xunit;

namespace BenchmarkDotNetAnalyser.Tests.Unit.Commands
{
    public class CommandExtensionsTests
    {
        private class NoOptionsStubCommand
        {
            [Description("Test")] public int Count { get; set; }
        }

        private class OptionsStubCommand
        {
            [Option(LongName = "test")] public int Count { get; set; }
            [Option(LongName = "verbose")] public bool Verbose { get; set; }
            public string Name { get; set; }
        }


        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void ToReturnCode_ValuesMapped(bool input, int expected)
        {
            var r = input.ToReturnCode();

            r.ShouldBe(expected);
        }

        [Theory]
        [InlineData(typeof(OptionsStubCommand), nameof(OptionsStubCommand.Count), "test")]
        [InlineData(typeof(OptionsStubCommand), nameof(OptionsStubCommand.Verbose), "verbose")]
        public void GetCommandOptionName_NameFound(Type commandType, string propName, string expected)
        {
            var cmd = CreateInstance(commandType);
            
            var name = cmd.GetCommandOptionName(propName);

            name.ShouldBe(expected);
        }

        [Theory]
        [InlineData(typeof(object), "Length")]
        [InlineData(typeof(OptionsStubCommand), "")]
        [InlineData(typeof(OptionsStubCommand), " ")]
        [InlineData(typeof(OptionsStubCommand), "zzz")]
        public void GetCommandOptionName_NameNotFound(Type commandType, string propName)
        {
            var cmd = CreateInstance(commandType);
            
            var name = cmd.GetCommandOptionName(propName);

            name.ShouldBeNull();
        }
        
        [Theory]
        [InlineData(nameof(NoOptionsStubCommand.Count))]
        [InlineData(nameof(OptionsStubCommand.Name))]
        public void GetCommandOptionName_NameFound_NoAttribute(string propName)
        {
            var cmd = new NoOptionsStubCommand() { Count = 999 };
            
            var name = cmd.GetCommandOptionName(propName);

            name.ShouldBeNull();
        }
        

        private object CreateInstance(Type objectType) => Activator.CreateInstance(objectType);
    }
}

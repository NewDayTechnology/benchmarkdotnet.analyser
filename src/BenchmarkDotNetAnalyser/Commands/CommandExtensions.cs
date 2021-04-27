using System;
using System.Diagnostics;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace BenchmarkDotNetAnalyser.Commands
{
    internal static class CommandExtensions
    {
        [DebuggerStepThrough]
        public static int ToReturnCode(this bool value) => value ? 0 : 1;
        
        [DebuggerStepThrough]
        public static string GetCommandOptionName(this object command, string propName)
        {
            command.ArgNotNull(nameof(command));
            propName.ArgNotNull(nameof(propName));

            var property = command.GetType().GetProperties()
                                   .Where(pi => StringComparer.Ordinal.Equals(pi.Name, propName))
                                   .Select(pi => pi.GetCustomAttributes(false)
                                                   .OfType<OptionAttribute>()
                                                   .FirstOrDefault() )
                                   .FirstOrDefault(a => a != null);
                
            return property?.LongName;
        }
    }
}

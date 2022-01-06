using StatlookkLogViewer.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StatlookLogViewer.Parser
{
    internal class LogParserTools
    {
        public static Dictionary<string, ILogParser> GetLogParserMap()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return (from objectType in assembly.GetTypes() where objectType.GetInterfaces().Contains(typeof(ILogParser)) select (ILogParser)Activator.CreateInstance(objectType)).ToDictionary(logParser => logParser.UniqueLogKey);
        }

        public static (ILogParser, string[]) DetectLogParser(string filePath)
        {
            var allFileLines = IOTools.ReadAllLines(filePath);

            foreach (var kvp in from kvp in GetLogParserMap() from line in allFileLines where line.Contains(kvp.Key) select kvp)
            {
                var logParser = kvp.Value;
                return (logParser, allFileLines);
            }

            return (null, allFileLines);

        }
    }
}

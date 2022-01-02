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
            var logParserMap = new Dictionary<string, ILogParser>();

            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type objectType in assembly.GetTypes())
            {

                if (objectType.GetInterfaces().Contains(typeof(ILogParser)))
                {
                    var logParser = (ILogParser)Activator.CreateInstance(objectType);

                    logParserMap.Add(logParser.UniqueLogKey, logParser);
                }
            }

            return logParserMap;
        }

        public static (ILogParser, string[]) DetectLogParser(string filePath)
        {
            string[] allFileLines = IOTools.ReadAllLines(filePath);

            ILogParser logParser = null;

            foreach (KeyValuePair<string, ILogParser> kvp in LogParserTools.GetLogParserMap())
            {
                foreach (string line in allFileLines)
                {
                    if (line.Contains(kvp.Key))
                    {
                        logParser = kvp.Value;
                        return (logParser, allFileLines);
                    }
                }
            }

            return (logParser, allFileLines);

        }
    }
}

using StatlookLogViewer.Model.Pattern;
using System.Collections.Generic;

namespace StatlookLogViewer.Parser
{
    internal class UpdateSchemaParser : ILogParser
    {
        public string UniqueLogKey { get; set; } = "Update Database Schema";

        public string StartLogGroupEntry { get; set; } = "	Start time:";

        public string EndLogGroupEntry { get; set; } = "	--------------------------------------------------------------------------------";

        public LogErrorPattern[] GetListOfErrors()
        {
            return new List<LogErrorPattern>()
            {
            }.ToArray();
        }

        public IEnumerable<LogPattern> GetLogPatterns()
        {
            return new List<LogPattern>()
            {
                new( "startTime", "	Start time:", true),
                new( "duration", "	Duration:", true),
                new( "conditionalExpressionResult", "	Conditional expression result:", true),
                new( "status", "	Status:", true)
             }.ToArray();
        }
    }
}

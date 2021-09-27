using StatlookLogViewer.Model.Pattern;
using System.Collections.Generic;
using System.Linq;

namespace StatlookLogViewer.Model
{
    public class LogPatternCollection
    {
        #region Members

        private readonly List<ILogPattern> _logPatterns = new List<ILogPattern>();

        #endregion Members

        #region Constructors

        public LogPatternCollection()
        {
            _logPatterns.AddRange(StatlookLogPattern.CreateLogPatterns());
            _logPatterns.AddRange(UsmLogPattern.CreateLogPatterns());
        }

        #endregion Constructors

        #region Methods

        public ILogPattern[] GetStatlookLogPatterns() => _logPatterns.Where(item => item is StatlookLogPattern).ToArray();

        public ILogPattern[] GetUsmLogPatterns() => _logPatterns.Where(item => item is UsmLogPattern).ToArray();

        public ILogPattern GetHeaderByKeyName(string keyName) => _logPatterns.Find(item => item.KeyName == keyName);

        #endregion Methods
    }
}

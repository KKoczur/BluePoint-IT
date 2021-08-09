using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatlookLogViewer
{
    public class DescriptorCollection
    {
        #region Members

        private List<Descriptor> _descriptors = new List<Descriptor>();

        #endregion Members

        #region Constructors

        public DescriptorCollection()
        {
            _descriptors.AddRange(CreateStatlookHeaders());
            _descriptors.AddRange(CreateUsmHeaders());
        }

        #endregion Constructors

        #region Methods

        public Descriptor[] GetStatlookHeaders() => _descriptors.Where(item => item.LogType == LogType.Statlook).ToArray();

        public Descriptor[] GetUsmHeaders() => _descriptors.Where(item => item.LogType == LogType.Usm).ToArray();

        public Descriptor GetHeaderByKeyName(string keyName) => _descriptors.Where(item => item.KeyName == keyName).FirstOrDefault();

        Descriptor[] CreateStatlookHeaders()
        {
            const LogType logType = LogType.Statlook;

            var result = new List<Descriptor>()
            {
                new Descriptor(logType,"uDate", "Date", true),
                new Descriptor(logType,"uLogger", " Logger:", true),
                new Descriptor(logType,"uType", " Type:", true),
                new Descriptor(logType,"uProcess", " Process ID:", true),
                new Descriptor(logType,"uThread", " Thread ID:", true),
                new Descriptor(logType,"uDescription", " Description:", true),
                new Descriptor(logType,"uException", " Exception:", true),
                new Descriptor(logType,"uMessage", "   Message:", true),
                new Descriptor(logType,"uMethod", "   Method:", true),
                new Descriptor(logType,"uStack", "   Stack:", true)
            };

            return result.ToArray();
        }

        private Descriptor[] CreateUsmHeaders()
        {
            const LogType logType = LogType.Usm;

            var result = new List<Descriptor>()
            {
                 new Descriptor(logType, "usmDate", "Date", true),
                new Descriptor(logType, "usmCode", " Code:", true),
                new Descriptor(logType, "usmType", " Type:", true),
                new Descriptor(logType, "usmSession", " Session:", true),
                new Descriptor(logType, "usmProcess", " Process ID:", true),
                new Descriptor(logType, "usmDescription", " Description:", true)
             };

            return result.ToArray();
        }

        #endregion Methods
    }
}

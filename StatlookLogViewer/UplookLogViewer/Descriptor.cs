
namespace StatlookLogViewer
{
    public class Descriptor
    {
        #region Constructors

        public Descriptor(string name)
        {
            Name = name;
        }

        public Descriptor(string name, string value)
            :this(name)
        {
            Value = value;
        }

        public Descriptor(string name, string value,bool show)
            :this (name, value)
        {
            Show = show;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Name of descriptor
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Value of descriptor
        /// </summary>
        public string Value { set; get; }

        /// <summary>
        /// Need to show
        /// </summary>
        public bool Show { set; get; }

        #endregion Properties

    }
}

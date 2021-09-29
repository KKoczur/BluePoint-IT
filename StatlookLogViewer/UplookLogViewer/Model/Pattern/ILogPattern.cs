﻿
namespace StatlookLogViewer.Model.Pattern
{
    public interface ILogPattern
    {
        /// <summary>
        /// Key name of descriptor
        /// </summary>
        public string KeyName { set; get; }

        /// <summary>
        /// Text pattern
        /// </summary>
        public string TextPattern { set; get; }

        /// <summary>
        /// Need to show
        /// </summary>
        public bool Show { set; get; }
    }
}

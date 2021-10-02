
using System;
using System.IO;
using System.Windows.Forms;

namespace StatlookkLogViewer.Tools
{
    public static class IOTools
    {
        public static string FormatFileSize(double fileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (fileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSize /= 1024;
            }

            return string.Format("{0:0.##} {1}", fileSize, sizes[order]);
        }

        /// <summary>
        /// Get file text content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>File text content</returns>
        public static string ReadAllFileText(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + exception.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get file text content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>File text content</returns>
        public static string[] ReadAllLines(string filePath)
        {
            try
            {
                return File.ReadAllLines(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + exception.Message);
                return Array.Empty<string>();
            }
        }

    }
}

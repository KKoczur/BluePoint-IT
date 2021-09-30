
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

    }
}

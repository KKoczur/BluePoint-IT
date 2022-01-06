using System;
using System.IO;
using System.Windows.Forms;

namespace StatlookkLogViewer.Tools;

public static class IOTools
{
    public static string FormatFileSize(double fileSize)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        var order = 0;
        while (fileSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            fileSize /= 1024;
        }

        return $"{fileSize:0.##} {sizes[order]}";
    }


    /// <summary>
    ///     Get file text content
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